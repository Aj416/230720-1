namespace Tigerspike.Solv.Services.Notification.Configuration
{
	public class MessengerOptions
	{
		/// <summary>
		/// Section name to be referred in app settings.
		/// </summary>
		public const string SectionName = "Messenger";
		public string BaseUrl { get; set; }
		public string AppId { get; set; }
		public string KeyId { get; set; }
		public string Secret { get; set; }
	}
}