using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class ChatActionFinalizedEventConsumer : IConsumer<IChatActionFinalizedEvent>
	{
		private readonly ITicketService _ticketService;
		private readonly IMediator _mediator;

		public ChatActionFinalizedEventConsumer(ITicketService ticketService, IMediator mediator)
		{
			_ticketService = ticketService;
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<IChatActionFinalizedEvent> context)
		{
			var notification = context.Message;

			Func<IChatActionFinalizedEvent, Task> handler = (ActionType)notification.Action.Type switch
			{
				ActionType.IsTicketSolvedQuestion => IsTicketSolvedQuestionHandler,
				ActionType.CSAT => CsatHandler,
				ActionType.NPS => NpsHandler,
				ActionType.Escalate => EscalateHandler,
				ActionType.FeedBack => FeedBackHandler,
				_ => throw new NotImplementedException($"Handler for {notification.Action.Type} is not implemented")
			};

			await handler(notification);
		}

		private async Task IsTicketSolvedQuestionHandler(IChatActionFinalizedEvent notification)
		{
			var isMarkedAsSolved = notification.Action.Options.Any(x => x.Value == bool.TrueString && x.IsSelected);
			var followupAction = isMarkedAsSolved ?
				_ticketService.Close(notification.ConversationId) :
				_ticketService.Reopen(notification.ConversationId);
			await followupAction;
		}

		private async Task CsatHandler(IChatActionFinalizedEvent notification)
		{
			var csatOption = notification.Action.Options.Single(x => x.IsSelected);
			var csat = int.Parse(csatOption.Value);
			await _ticketService.SetCsat(notification.ConversationId, csat);
		}

		private async Task NpsHandler(IChatActionFinalizedEvent notification)
		{
			var npsOption = notification.Action.Options.Single(x => x.IsSelected);
			var nps = int.Parse(npsOption.Value);
			await _ticketService.SetNps(notification.ConversationId, nps);
		}

		private async Task EscalateHandler(IChatActionFinalizedEvent notification)
		{
			var option = notification.Action.Options.Single(x => x.IsSelected);
			var level = Enum.Parse<TicketLevel>(option.Value);
			var cmd = new EscalateTicketCommand(notification.ConversationId, TicketEscalationReason.Customer, level);
			await _mediator.Send(cmd);
		}

		private async Task FeedBackHandler(IChatActionFinalizedEvent notification)
		{
			if (!string.IsNullOrEmpty(notification.Content))
			{
				await _ticketService.SetAdditionalFeedBack(notification.ConversationId, notification.Content);
			}
			else
			{
				var cmd = new SkipTicketAdditionalFeedBackCommand(notification.ConversationId);
				await _mediator.Send(cmd);
			}
		}
	}
}