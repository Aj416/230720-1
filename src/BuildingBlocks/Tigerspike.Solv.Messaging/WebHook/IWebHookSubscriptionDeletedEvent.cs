using System;

namespace Tigerspike.Solv.Messaging.WebHook
{
	public interface IWebHookSubscriptionDeletedEvent
	{
		/// <summary>
		/// Gets or sets the Brand id.
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// Gets or sets the webhook subscription id.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Gets or sets the event timestamp.
		/// </summary>
		public DateTime Timestamp { get; }
	}
}