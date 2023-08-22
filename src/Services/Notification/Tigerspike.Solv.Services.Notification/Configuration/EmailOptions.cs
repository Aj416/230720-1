namespace Tigerspike.Solv.Services.Notification.Configuration
{
	public class EmailOptions
	{
		public const string SectionName = "Email";

		public string Host { get; set; }

		public string Port { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string DefaultEmail { get; set; }

		public string TicketEmail { get; set; }

		public string DisplayName { get; set; }
	}
}
