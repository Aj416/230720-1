using System.ComponentModel.DataAnnotations;
using Tigerspike.Solv.Application.Enums;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Create WebHook Model
	/// </summary>
	public class CreateWebHookModel
	{
		/// <summary>
		/// Event type that webhook is subcribed for
		/// </summary>
		[Required]
		public WebHookEventTypes EventType { get; set; }

		/// <summary>
		/// Webhook receiver url
		/// </summary>
		[Required]
		public string Url { get; set; }

		/// <summary>
		/// Webhook notification body
		/// </summary>
		[Required]
		public string Body { get; set; }

		/// <summary>
		/// Webhook notification verb (e.g. POST or PUT)
		/// </summary>
		[Required]
		public string Verb { get; set; }

		/// <summary>
		/// Webhook notification content type
		/// </summary>
		[Required]
		public string ContentType { get; set; }

		/// <summary>
		/// Secret shared between client and webhook sender to generate signature for the payload
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// Authorization header content that will be provided on each notification
		/// </summary>
		public string Authorization { get; set; }
	}
}