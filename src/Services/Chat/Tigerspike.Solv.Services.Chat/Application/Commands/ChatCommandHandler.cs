using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Application.Services;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Chat.Application.Events;

namespace Tigerspike.Solv.Chat.Application.CommandHandlers
{
	public class ChatCommandHandler :
		IRequestHandler<MarkConversationAsReadCommand, Unit>
	{
		private readonly IChatRepository _chatRepository;
		private readonly IChatService _chatService;
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<ChatCommandHandler> _logger;

		public ChatCommandHandler(
			IChatRepository chatRepository,
			IChatService chatService,
			IMediatorHandler mediator,
			ILogger<ChatCommandHandler> logger)
		{
			_chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
			_chatService = chatService;
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
		{
			var conversation = _chatRepository.GetConversations(request.ConversationId).SingleOrDefault();

			if (conversation == null)
			{
				_logger.LogError($"Conversation with id {request.ConversationId} could not be found.");
				return Unit.Value;
			}

			conversation.LastReadMessageTimeStamp = request.TimeStamp;

			_chatRepository.AddOrUpdateConversation(conversation);

			await _mediator.RaiseEvent(new ConversationMarkedAsReadEvent(request.ConversationId));

			return Unit.Value;
		}
	}
}