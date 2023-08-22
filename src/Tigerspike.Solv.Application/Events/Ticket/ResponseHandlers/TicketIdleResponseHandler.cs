using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketIdleResponseHandler :
		INotificationHandler<TicketAcceptedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly BrandAdvocateResponseType[] responseTypes = new[] { BrandAdvocateResponseType.TicketCreated };

		public TicketIdleResponseHandler(ITicketAutoResponseService ticketAutoResponseService)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			await _ticketAutoResponseService.CancelResponses(notification.BrandId, notification.TicketId, responseTypes);
		}

	}
}