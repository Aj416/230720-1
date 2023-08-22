using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketOpenTimeoutEscalationResponseHandler :
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketSolvedEvent>

	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly BrandAdvocateResponseType[] responseTypes = new[] { BrandAdvocateResponseType.TicketOpenTimeoutEscalation };

		public TicketOpenTimeoutEscalationResponseHandler(ITicketAutoResponseService ticketAutoResponseService) => _ticketAutoResponseService = ticketAutoResponseService;

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.Level == TicketLevel.Regular)
			{
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, advanceScheduleBy: notification.NewStatusDuration);
			}
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken) => await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken) => await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken) => await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);
	}
}