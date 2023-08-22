namespace Tigerspike.Solv.Application.Models
{
	public class EmailTemplatesOptions
	{
		public const string SectionName = "EmailTemplates";

		public string AdvocateAcceptedEmailSubject { get; set; }
		public string EmailLogoLocation { get; set; }
		public string AdvocateAcceptedEmailIllustrationLocation { get; set; }
		public string AdvocateExportEmailAttachmentContentType { get; set; }
		public string AdvocateExportEmailAttachmentFileName { get; set; }
		public string AdvocateExportEmailSubject { get; set; }
		public string AdvocateDeleteEmailSubject { get; set; }
		public string AdvocateDeleteUrl { get; set; }
		public string AdvocateDeleteUrlQueryParamEmail { get; set; }
		public string AdvocateDeleteUrlQueryParamKey { get; set; }
		public string AdvocateSignUpUrl { get; set; }
		public string AdvocateProfilingUrl { get; set; }
		public string AdvocateApplicationCreatedEmailSubject { get; set; }
		public string BrandsBlockedEmailSubject { get; set; }
		public string ChatUrl { get; set; }

		/// <summary>
		/// The url to close-ticket page on the FE.
		/// </summary>
		public string RateUrl { get; set; }

		public string ConsoleUrl { get; set; }

		/// <summary>
		/// Number of seconds to schedule the reminder of filling out profiling for the new advocate application
		/// </summary>
		public int ProfilingReminderDelaySeconds { get; set; }

		/// <summary>
		/// Number of seconds to schedule the reminder that solver has put a response on a customers chat
		/// </summary>
		/// <value></value>
		public int ChatReminderDelaySeconds { get; set; }

		/// <summary>
		/// How long to wait before sending the customer a reminder that the ticket is Solved
		/// and requires an answer (yes/no)
		/// </summary>
		public int CloseTicketReminderDelayMinutes { get; set; }

		public string MarketingSiteAuthenticatorAppUrl { get; set; }

		/// <summary>
		/// The url to close-ticket page for end-chat feature.
		/// </summary>
		public string EndChatUrl { get; set; }
	}
}