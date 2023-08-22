using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class TicketEscalationEventHandler :	INotificationHandler<TicketAbandonedEvent>
	{
		private readonly IMediatorHandler _mediator;

		public TicketEscalationEventHandler(IMediatorHandler mediator)
		{
			_mediator = mediator;
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.Action == TicketFlowAction.Escalate)
			{
				await _mediator.SendCommand(new EscalateTicketCommand(notification.TicketId, TicketEscalationReason.AbandonReason));
			}
		}

	}
}