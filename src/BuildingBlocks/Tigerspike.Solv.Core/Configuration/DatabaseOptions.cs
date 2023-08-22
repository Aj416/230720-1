namespace Tigerspike.Solv.Core.Configuration
{
	public class DatabaseOptions
	{
		public const string SectionName = "DatabaseSettings";

		public string Server { get; set; }

		public uint Port { get; set; }

		public string User { get; set; }

		public string Password { get; set; }

		public string Database { get; set; }
	}
}