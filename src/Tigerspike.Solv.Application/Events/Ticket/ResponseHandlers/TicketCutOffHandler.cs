using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketCutOffHandler :
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketEscalatedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly BrandAdvocateResponseType[] responseTypes = new[] { BrandAdvocateResponseType.TicketCutOff };

		public TicketCutOffHandler(ITicketAutoResponseService ticketAutoResponseService)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.Level == TicketLevel.PushedBack)
			{
				await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);
			}
		}

	}
}