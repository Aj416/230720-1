using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Messaging.WebHook;
using Tigerspike.Solv.Services.WebHook.Enums;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.WebHook.Application.Consumers
{
	/// <summary>
	/// This consumer should be replaced by integration events triggered from the Ticket service.
	/// </summary>
	public class StartWebHookTriggerConsumer : IConsumer<IStartWebHookTriggerCommand>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ILogger<StartWebHookTriggerConsumer> _logger;

		public StartWebHookTriggerConsumer(
			ISubscriptionRepository subscriptionRepository,
			ILogger<StartWebHookTriggerConsumer> logger)
		{
			_subscriptionRepository = subscriptionRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IStartWebHookTriggerCommand> context)
		{
			_logger.LogInformation("Checking webhook subscriptions for brand {0}, and event type {1}",
				context.Message.BrandId, context.Message.EventType);

			var request = context.Message;
			var subscriptions =
				await _subscriptionRepository.GetSubscriptions(request.BrandId, request.EventType);

			foreach (var sub in subscriptions)
			{
				_logger.LogInformation("Sending webhook command for brand {0}, event type {1} and subscription {2}",
					context.Message.BrandId, context.Message.EventType, sub.Id);

				await context.Send<ISendWebHookCommand>(new {sub.Url, sub.Body, sub.Verb,
					sub.ContentType, sub.Authorization, sub.Secret,
					Data = request.Payload});
			}
		}
	}
}