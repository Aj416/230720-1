using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.WebHook
{
	public interface ISendWebHookCommand
	{
		/// <summary>
		/// Endpoint to call with the payload
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Content for the Authorization header
		/// </summary>
		public string Authorization { get; set; }

		/// <summary>
		/// Http verb type for the notification call
		/// </summary>
		/// <value>POST or PUT</value>
		public string Verb { get; set; }

		/// <summary>
		/// ContentType header
		/// </summary>
		/// <value>application/json</value>
		public string ContentType { get; set; }

		/// <summary>
		/// Notification body
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Secret to use for generating payload signature
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// Payload
		/// </summary>
		public IDictionary<string, object> Data { get; set; }
	}
}