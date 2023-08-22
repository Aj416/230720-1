using System;

namespace Tigerspike.Solv.Messaging.WebHook
{
	public interface ICreateSubscriptionCommand
	{
		/// <summary>
		/// Brand for which the webhook should be registered
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Owner of the webhook
		/// </summary>
		public Guid? UserId { get; set; }

		/// <summary>
		/// Endpoint to call with the payload
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Webhook notification body
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Webhook notification verb (e.g. POST or PUT)
		/// </summary>
		public string Verb { get; set; }

		/// <summary>
		/// Webhook notification content type
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Secret to use for generating payload signature
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// Authorization header content that will be provided on each notification
		/// </summary>
		public string Authorization { get; set; }

		/// <summary>
		/// Event to subscribe on
		/// </summary>
		public int EventType { get; set; }
	}
}