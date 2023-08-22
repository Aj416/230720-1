using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Z.EntityFramework.Plus;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class SetPricingForNewTicketsCommandHandler : CommandHandler, IRequestHandler<SetPricingForNewTicketsCommand, int>
	{
		private readonly ITimestampService _timestampService;
		private readonly SolvDbContext _dbContext;

		public SetPricingForNewTicketsCommandHandler(
			ITimestampService timestampService,
			SolvDbContext dbContext,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_timestampService = timestampService;
			_dbContext = dbContext;
		}

		public async Task<int> Handle(SetPricingForNewTicketsCommand request, CancellationToken cancellationToken)
		{
			var timestamp = _timestampService.GetUtcTimestamp();
			var query = _dbContext.Set<Ticket>().AsQueryable() // bulk update all eligible tickets
				.Where(x => x.Status == TicketStatusEnum.New || (x.Status == TicketStatusEnum.Reserved && x.ReservationExpiryDate < DateTime.UtcNow)) // update new tickets ...
				.Where(x => x.IsPractice == false) // .. ignore practice tickets ..
				.Where(x => x.BrandId == request.BrandId) // ... from specified brand
				.Where(x => request.TicketIdList == null || request.TicketIdList.Contains(x.Id)); // ... and maybe limit to just certain scope (in particular only when ticket that just faced rejection)

			var changedTickets = await query.Select(x => x.Id).ToListAsync();
			var affectedItemsCount = await query
				.UpdateAsync(x => new Ticket
				{
					Price = request.Price,
					Fee = request.Fee,
					ModifiedDate = timestamp,
				});

			foreach (var ticketId in changedTickets)
			{
				await _mediator.RaiseEvent(new TicketPriceChangedEvent(ticketId, request.Price, request.Fee, false));
			}

			return affectedItemsCount;
		}
	}
}