using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.User;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	/// <summary>
	///  The ticket event handler in case an advocate is blocked
	/// </summary>
	public class AdvocateBlockedEventHandler :
		INotificationHandler<UserBlockedEvent>,
		INotificationHandler<AdvocateBrandsRemovedEvent>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IMediatorHandler _mediator;

		public AdvocateBlockedEventHandler(ITicketRepository ticketRepository, IMediatorHandler mediator, IBrandRepository brandRepository)
		{
			_ticketRepository = ticketRepository;
			_brandRepository = brandRepository;
			_mediator = mediator;
		}

		public Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken) => AbandonTickets(notification.UserId, null);

		public Task Handle(AdvocateBrandsRemovedEvent notification, CancellationToken cancellationToken) => AbandonTickets(notification.AdvocateId, notification.BrandIds);

		private async Task AbandonTickets(Guid advocateId, IEnumerable<Guid> brandIds)
		{
			IList<(Guid Id, Guid BrandId)> ticketsIds = await _ticketRepository.GetAssignedTickets(advocateId, t => Tuple.Create(t.Id, t.BrandId).ToValueTuple());
			var ticketsPerBrand = ticketsIds
				.GroupBy(x => x.BrandId)
				.Where(x => brandIds == null || brandIds.Contains(x.Key))
				.ToDictionary(x => x.Key, x => x.Select(y => y.Id).ToList());

			foreach (var item in ticketsPerBrand)
			{
				var abandonReasons = await _brandRepository.GetFirstOrDefaultAsync(x => x.AbandonReasons, x => x.Id == item.Key);
				var blockedReasons = abandonReasons
					.Where(x => x.IsBlockedAdvocate)
					.Select(x => x.Id)
					.ToArray();

				foreach (var ticketId in item.Value)
				{
					await _mediator.SendCommand(new AbandonTicketCommand(ticketId, blockedReasons));
				}
			}
		}

	}
}
