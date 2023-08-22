using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Search.Interfaces;

namespace Tigerspike.Solv.Application.Consumers
{
	public class UpdateSearchIndexWhenChatMessageAddedEventConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly ISearchService<TicketSearchModel> _searchService;
		private readonly ILogger<UpdateSearchIndexWhenChatMessageAddedEventConsumer> _logger;

		public UpdateSearchIndexWhenChatMessageAddedEventConsumer(
			ISearchService<TicketSearchModel> searchService, ILogger<UpdateSearchIndexWhenChatMessageAddedEventConsumer> logger)
		{
			_searchService = searchService;
			_logger = logger;
		}

		public Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			_logger.LogInformation("Updating ticket {0} after receiving a chat message {1}",
				notification.ConversationId, notification.Message);

			if (notification.MessageType != (int)MessageType.Message)
			{
				return Task.CompletedTask;
			}

			if (notification.SenderType == (int)UserType.Advocate)
			{
				return _searchService.Update(notification.ConversationId,
					new {LastAdvocateMessageDate = notification.CreatedDate});
			}
			else if (notification.SenderType == (int)UserType.Customer)
			{
				return _searchService.Update(notification.ConversationId,
					new {LastCustomerMessageDate = notification.CreatedDate});
			}

			return Task.CompletedTask;
		}
	}
}