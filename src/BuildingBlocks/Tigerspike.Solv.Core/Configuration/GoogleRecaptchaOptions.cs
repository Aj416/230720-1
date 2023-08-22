namespace Tigerspike.Solv.Core.Configuration
{
	public class GoogleRecaptchaOptions
	{
		public const string SectionName = "GoogleRecaptcha";

		public string SecretKey { get; set; }

		public string TestValue { get; set; }

		public string Timeout { get; set; }

		public bool EnableRecpatcha { get; set; }
	}
}