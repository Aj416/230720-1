using System;

namespace Tigerspike.Solv.Messaging.IdentityVerification
{
	public interface ICreateIdentityCheckCommand
	{
		/// <summary>
		/// The solver id.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The existing applicant id.
		/// </summary>
		public string ApplicantId { get; set; }

		/// <summary>
		/// The solver first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// The solver last name.
		/// </summary>
		public string LastName { get; set; }
	}
}