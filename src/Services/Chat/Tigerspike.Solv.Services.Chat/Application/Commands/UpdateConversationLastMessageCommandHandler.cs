using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Chat.Application.CommandHandlers
{
	public class UpdateConversationLastMessageCommandHandler : AsyncRequestHandler<UpdateConversationLastMessageCommand>
	{
		private readonly IChatRepository _chatRepository;
		private readonly ILogger<UpdateConversationLastMessageCommandHandler> _logger;

		public UpdateConversationLastMessageCommandHandler(IChatRepository chatRepository,
			ILogger<UpdateConversationLastMessageCommandHandler> logger)
		{
			_chatRepository = chatRepository;
			_logger = logger;
		}

		protected override Task Handle(UpdateConversationLastMessageCommand request, CancellationToken cancellationToken)
		{
			// TODO: Check if the message coming from advocate not from other senders.
			var conversation = _chatRepository.GetConversations(request.ConversationId).SingleOrDefault();

			if(conversation == null)
			{
				_logger.LogError($"Conversation with id {request.ConversationId} could not be found.");
				return Task.CompletedTask;
			}

			conversation.LastMessageTimeStamp = request.TimeStamp;

			_chatRepository.AddOrUpdateConversation(conversation);

			return Task.CompletedTask;
		}
	}
}