using System;
using Tigerspike.Solv.Application.Enums;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// WebHook View Model
	/// </summary>
	public class WebHookModel
	{
		/// <summary>
		/// Id of the webhook
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Event type that webhook is subcribed for
		/// </summary>
		public WebHookEventTypes EventType { get; set; }

		/// <summary>
		/// Webhook receiver url
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
	}
}