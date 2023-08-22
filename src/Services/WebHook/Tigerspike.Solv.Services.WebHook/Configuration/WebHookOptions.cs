using System;

namespace Tigerspike.Solv.Services.WebHook.Configuration
{
	public class WebHookOptions
	{
		/// <summary>
		/// Configuration section name.
		/// </summary>
		public const string SectionName = "WebHook";

		/// <summary>
		/// User agent.
		/// </summary>
		public string UserAgent { get; set; } = "Solv-WebHook";

		/// <summary>
		/// Duration after which timeout happens.
		/// </summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
	}
}