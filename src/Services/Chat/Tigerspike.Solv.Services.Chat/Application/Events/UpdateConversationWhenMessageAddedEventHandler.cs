using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Core.Mediator;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class UpdateConversationWhenMessageAddedEventHandler : INotificationHandler<MessageAddedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly IChatRepository _chatRepository;
		private readonly ILogger<UpdateConversationWhenMessageAddedEventHandler> _logger;

		public UpdateConversationWhenMessageAddedEventHandler(
			IChatRepository chatRepository,
			IMediatorHandler mediator,
			ILogger<UpdateConversationWhenMessageAddedEventHandler> logger)
		{
			_chatRepository = chatRepository;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Handle(MessageAddedEvent notification, CancellationToken cancellationToken)
		{
			var conversationId = notification.Message.ConversationId;
			var conversation = _chatRepository.GetConversation(conversationId);

			if (conversation == null)
			{
				_logger.LogError(
					$"Could not find conversation {notification.Message.ConversationId} when updating conversation after MessageAddedEvent event");
				return;
			}

			_logger.LogInformation($"Updating conversation last message for conversation {0}", notification.ConversationId);

			await _mediator.SendCommand(new UpdateConversationLastMessageCommand(notification.Message.ConversationId,
				notification.Timestamp));
		}
	}
}