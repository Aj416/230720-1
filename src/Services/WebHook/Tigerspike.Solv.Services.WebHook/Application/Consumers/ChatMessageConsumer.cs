using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Messaging.Chat;
using Tigerspike.Solv.Messaging.WebHook;
using Tigerspike.Solv.Services.WebHook.Enums;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.WebHook.Application.Consumers
{
	/// <summary>
	/// The chat messages consumer
	/// </summary>
	public class ChatMessageConsumer : IConsumer<IChatMessageAddedEvent>
	{
		#region Private Properties

		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<ChatMessageConsumer> _logger;

		/// <summary>
		/// The subscription repository
		/// </summary>
		private readonly ISubscriptionRepository _subscriptionRepository;
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="subscriptionRepository"></param>
		public ChatMessageConsumer(ILogger<ChatMessageConsumer> logger, ISubscriptionRepository subscriptionRepository)
		{
			_logger = logger;
			_subscriptionRepository = subscriptionRepository;
		}
		#endregion

		#region Consume(ConsumeContext<IChatMessageAddedEvent> context)

		/// <summary>
		/// Consumer for IChatMessageAddedEvent
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var request = context.Message;
			var eventType = WebHookEventTypes.ChatMessageAddedEvent;
			
			_logger.LogInformation("Checking webhook subscriptions for brand {0}, and event type {1}",
				request.BrandId, eventType);
			
			var subscriptions = await _subscriptionRepository.GetSubscriptions(request.BrandId, (int) eventType);

			foreach (var sub in subscriptions)
			{
				_logger.LogInformation("Sending webhook command for brand {0}, event type {1} and subscription {2}",
					request.BrandId, eventType, sub.Id);
				
				//data to be passed onto the webhook
				var payload = new Dictionary<string, object>
				{
					{ "Id", request.Id },
					{ "MessageId", request.ClientMessageId },
					{ "Message", request.Message },
					{ "MessageType", (MessageType) request.MessageType },
					{ "Date", request.CreatedDate },
					{ "RelevantTo", request.RelevantTo.Select(x => { return (UserType) x; }).ToList() },
					{ "Sender" , (UserType) request.SenderType }
				};
				
				await context.Send<ISendWebHookCommand>(new { sub.Url, sub.Body, sub.Verb,
					sub.ContentType, sub.Authorization, sub.Secret,	Data = payload });
			}
		}
		#endregion
	}
}