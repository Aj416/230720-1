using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Chat.Application.Commands
{
	public class UpdateAdvocateForConversationHandler : IRequestHandler<UpdateAdvocateForConversation>
	{
		private readonly IChatRepository _chatRepository;
		private readonly ILogger<UpdateAdvocateForConversationHandler> _logger;

		public UpdateAdvocateForConversationHandler(IChatRepository chatRepository,
			ILogger<UpdateAdvocateForConversationHandler> logger)
		{
			_chatRepository = chatRepository;
			_logger = logger;
		}

		public Task<Unit> Handle(UpdateAdvocateForConversation request, CancellationToken cancellationToken)
		{
			var conversation = _chatRepository.GetConversation(request.ConversationId);

			if (conversation == null)
			{
				_logger.LogError("Conversation is not found {0} with status {1}", request.ConversationId,
					request.Status);
				return Task.FromResult(Unit.Value);
			}

			_logger.LogInformation($"Updating conversation {conversation.Id} advocate");

			conversation.AdvocateId = request.AdvocateId?.ToString();
			conversation.AdvocateFirstName = request.AdvocateFirstName;
			conversation.AdvocateCsat = request.AdvocateCsat ?? 0;

			_chatRepository.AddOrUpdateConversation(conversation);

			_logger.LogInformation($"Updated conversation {conversation.Id} advocate");

			return Task.FromResult(Unit.Value);
		}
	}
}