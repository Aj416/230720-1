using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Messaging.WebHook;
using Tigerspike.Solv.Services.WebHook.Application.IntegrationEvents;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.WebHook.Application.Consumers
{
	public class DeleteSubscriptionConsumer : IConsumer<IDeleteSubscriptionCommand>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ILogger<DeleteSubscriptionConsumer> _logger;

		public DeleteSubscriptionConsumer(
			ISubscriptionRepository subscriptionRepository,
			ILogger<DeleteSubscriptionConsumer> logger)
		{
			_subscriptionRepository = subscriptionRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IDeleteSubscriptionCommand> context)
		{
			var request = context.Message;

			if (request.BrandId == Guid.Empty || request.Id == Guid.Empty)
			{
				_logger.LogError("Cannot process delete webhook subscription request", request);

				return;
			}

			var subscription = _subscriptionRepository.GetSubscription(request.BrandId, request.Id);
			if (subscription != null)
			{
				try
				{
					_subscriptionRepository.DeleteSubscription(request.BrandId, request.Id);

					// Publish an integration event
					_logger.LogInformation($"Publishing webhook subscription deleted event {subscription.Id}");

					await context.Publish<IWebHookSubscriptionDeletedEvent>(
						new WebHookSubscriptionDeletedEvent(subscription.BrandId, subscription.Id));
				}
				catch (Exception ex)
				{
					_logger.LogError("Failed to delete a webhook subscription", ex);
				}
			}
			else
			{
				_logger.LogError("Webhook subscription not found", request);
			}
		}
	}
}