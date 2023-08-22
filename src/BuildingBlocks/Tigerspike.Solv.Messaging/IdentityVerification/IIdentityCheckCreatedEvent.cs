using System;

namespace Tigerspike.Solv.Messaging.IdentityVerification
{
	public interface IIdentityCheckCreatedEvent
	{
		/// <summary>
		/// The advocate id.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The applicant id.
		/// </summary>
		public string ApplicantId { get; set; }

		/// <summary>
		/// The check id.
		/// </summary>
		public string CheckId { get; set; }

		/// <summary>
		/// The report url.
		/// </summary>
		public string CheckReportUrl { get; set; }

		/// <summary>
		/// The event timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }
	}
}