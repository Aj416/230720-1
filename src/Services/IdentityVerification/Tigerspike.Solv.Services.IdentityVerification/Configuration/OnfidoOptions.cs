using Tigerspike.Solv.Services.IdentityVerification.Refit;

namespace Tigerspike.Solv.Services.IdentityVerification.Configuration
{
	public class OnfidoOptions
	{
		public const string SectionName = "Onfido";

		/// <summary>
		/// The url of API.
		/// </summary>
		public string ApiUrl { get; set; }

		/// <summary>
		/// The authorization token for API
		/// </summary>
		public string ApiToken { get; set; }

		/// <summary>
		/// The Web SDK referrer to supply for generating JWT token
		/// </summary>
		public string Referrer { get; set; }

		/// <summary>
		/// The security token for authorizing webhook notifications
		/// </summary>
		public string WebhookToken { get; set; }

		/// <summary>
		/// The list of reports to request while doing a applicant check
		/// </summary>
		public ReportType[] Reports { get; set; }
	}
}