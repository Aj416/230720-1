using System;
using Tigerspike.Solv.Messaging.WebHook;

namespace Tigerspike.Solv.Services.WebHook.Application.IntegrationEvents
{
	public class WebHookSubscriptionCreatedEvent : IWebHookSubscriptionCreatedEvent
	{
		public Guid BrandId { get; private set; }
		public Guid Id { get; private set; }
		public DateTime Timestamp { get; private set; }

		public WebHookSubscriptionCreatedEvent(Guid brandId, Guid id)
		{
			BrandId = brandId;
			Id = id;
			Timestamp = DateTime.UtcNow;
		}
	}
}