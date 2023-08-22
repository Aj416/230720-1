using System;

namespace Tigerspike.Solv.Messaging.IdentityVerification
{
	public interface IIdentityCheckCompletedEvent
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
		/// Whether the check succeeded of failed.
		/// </summary>
		public bool? Success { get; set; }

		/// <summary>
		/// The event timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }
	}
}