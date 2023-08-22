using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketAbandonedResponseHandler : INotificationHandler<TicketAbandonedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;

		private readonly BrandAdvocateResponseType[] responseTypes = new[] { BrandAdvocateResponseType.TicketAbandoned, BrandAdvocateResponseType.TicketAbandonedEscalation };
		public TicketAbandonedResponseHandler(ITicketAutoResponseService ticketAutoResponseService)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.Action != TicketFlowAction.Escalate)
			{
				var model = new BrandResponseTemplateModel(notification.AdvocateFirstName);
				await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model, abandonedCount: notification.AbandonedCount, isAutoAbandoned: notification.AutoAbandoned);
			}
		}
	}
}