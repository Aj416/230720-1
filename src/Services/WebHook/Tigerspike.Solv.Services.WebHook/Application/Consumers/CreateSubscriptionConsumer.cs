using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Messaging.WebHook;
using Tigerspike.Solv.Services.WebHook.Application.IntegrationEvents;
using Tigerspike.Solv.Services.WebHook.Enums;
using Tigerspike.Solv.Services.WebHook.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.WebHook.Application.Consumers
{
	public class CreateSubscriptionConsumer : IConsumer<ICreateSubscriptionCommand>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ILogger<CreateSubscriptionConsumer> _logger;

		public CreateSubscriptionConsumer(
			ISubscriptionRepository subscriptionRepository,
			ILogger<CreateSubscriptionConsumer> logger)
		{
			_subscriptionRepository = subscriptionRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ICreateSubscriptionCommand> context)
		{
			var request = context.Message;

			if (request.BrandId == Guid.Empty ||
			    string.IsNullOrWhiteSpace(request.Url) ||
			    !Enum.IsDefined(typeof(WebHookEventTypes), request.EventType))
			{
				_logger.LogError("Cannot process create webhook subscription request", request);

				return;
			}

			var subscription = new Domain.Subscription(request.BrandId, request.EventType, request.UserId, request.Url,
				request.Body, request.Verb, request.ContentType, request.Secret, request.Authorization);

			try
			{
				_subscriptionRepository.AddOrUpdateSubscription(subscription);

				// Publish an integration event
				_logger.LogInformation($"Publishing webhook subscription create event {subscription.Id}");

				await context.Publish<IWebHookSubscriptionCreatedEvent>(
					new WebHookSubscriptionCreatedEvent(subscription.BrandId, subscription.Id));
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to create a new webhook subscription", ex);
			}
		}
	}
}