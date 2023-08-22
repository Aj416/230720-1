using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketPricingEventHandler :
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketRejectedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IBrandService _brandService;

		public TicketPricingEventHandler(
			ITicketRepository ticketRepository,
			IBrandRepository brandRepository,
			IBrandService brandService,
			IMediatorHandler mediator)
		{
			_ticketRepository = ticketRepository;
			_brandRepository = brandRepository;
			_brandService = brandService;
			_mediator = mediator;
		}

		public Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			return SetPricingForTicket(notification.TicketId);
		}

		public Task Handle(TicketRejectedEvent notification, CancellationToken cancellationToken)
		{
			return SetPricingForTicket(notification.TicketId);
		}

		private async Task SetPricingForTicket(Guid ticketId)
		{
			var ticket = await _ticketRepository.FindAsync(ticketId);
			var brand = await _brandRepository.FindAsync(ticket.BrandId);
			var desiredFee = _brandService.CalculateTicketFee(brand.TicketPrice, brand.FeePercentage);
			if (ticket.Price != brand.TicketPrice || ticket.Fee != desiredFee)
			{
				await _mediator.SendCommand(new SetPricingForNewTicketsCommand(brand.Id, brand.TicketPrice, desiredFee,
					new[] {ticket.Id}));
			}
		}
	}
}