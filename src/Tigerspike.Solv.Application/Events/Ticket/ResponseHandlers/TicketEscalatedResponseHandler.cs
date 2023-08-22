using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketEscalatedResponseHandler : INotificationHandler<TicketEscalatedEvent>
	{

		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly BrandAdvocateResponseType[] responseTypes = new[] { BrandAdvocateResponseType.TicketEscalated };

		public TicketEscalatedResponseHandler(ITicketAutoResponseService ticketAutoResponseService)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			var model = new BrandResponseTemplateModel(notification.AdvocateFirstName);
			await _ticketAutoResponseService.SendResponses(notification.BrandId, notification.TicketId, responseTypes, model: model, escalationReason: notification.EscalationReason);
		}
	}
}