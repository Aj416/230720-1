using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers
{
	public class ChatAutoResponseCompletedEventConsumer : IConsumer<IChatAutoResponseCompletedEvent>
	{
		private readonly IMediatorHandler _mediator;

		public ChatAutoResponseCompletedEventConsumer(IMediatorHandler mediator)
		{
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<IChatAutoResponseCompletedEvent> context)
		{
			Command<Unit> command;
			switch ((BrandAdvocateResponseType)context.Message.ResponseType)
			{
				case BrandAdvocateResponseType.NotificationResumptionTicketAbandoned:
					command = new AbandonTicketCommand(context.Message.ConversationId, true);
					break;
				case BrandAdvocateResponseType.NotificationResumptionTicketInProgress:
				case BrandAdvocateResponseType.NotificationResumptionTicketReopened:
					command = new SetNotificationResumptionStateCommand(context.Message.ConversationId,
						NotificationResumptionState.CustomerNotified);
					break;
				case BrandAdvocateResponseType.ReturningCustomerTicketAbandoned:
					command = new AbandonTicketCommand(context.Message.ConversationId, true);
					break;
				case BrandAdvocateResponseType.ReturningCustomerTicketOpen:
				case BrandAdvocateResponseType.ReturningCustomerTicketInProgress:
				case BrandAdvocateResponseType.ReturningCustomerTicketReopened:
					command = new SetReturningCustomerStateCommand(context.Message.ConversationId,
						ReturningCustomerState.CustomerNotified);
					break;
				case BrandAdvocateResponseType.TicketCutOff:
					command = new CloseTicketCommand(context.Message.ConversationId, ClosedBy.CutOff);
					break;
				case BrandAdvocateResponseType.TicketAbandonedEscalation:
					command = new EscalateTicketCommand(context.Message.ConversationId,
						TicketEscalationReason.AbandonedCountExceeded);
					break;
				case BrandAdvocateResponseType.TicketOpenTimeoutEscalation:
					command = new EscalateTicketCommand(context.Message.ConversationId,
						TicketEscalationReason.OpenTimeExceeded);
					break;
				default:
					command = null;
					break;
			}

			if (command != null)
			{
				await _mediator.SendCommand(command);
			}
		}
	}
}