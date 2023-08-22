using System;
using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class AdvocateModel
	{
		public Guid Id { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
		public string FullName { get; set; }

		public string Phone { get; set; }

		public bool InternalAgent { get; set; }

		public bool PaymentMethodSetup { get; set; }

		public bool PaymentEmailVerified { get; set; }

		public bool Enabled { get; set; }

		public bool VideoWatched { get; set; }

		public bool Practicing { get; set; }

		public bool PracticeComplete { get; set; }

		public string PaymentAccountId { get; set; }

		public AdvocateStatus Status { get; set; }

		public string CountryCode { get; set; }

		public string State { get; set; }

		/// <summary>
		/// Indicates whether the user has seen the notification for new unlocked brands and dismissed it.
		/// </summary>
		public bool? ShowBrandNotification { get; set; }

		/// <summary>
		/// The average Csat that this advocate has received on all tickets he solved.
		/// </summary>
		public decimal Csat { get; set; }

		/// <summary>
		/// Identity verification status
		/// </summary>
		public IdentityVerificationStatus IdentityVerificationStatus { get; set; }

		/// <summary>
		/// Identity applicant id
		/// </summary>
		public string IdentityApplicantId { get; set; }

		/// <summary>
		/// The url of the identity check result url.
		/// </summary>
		public string IdentityCheckResultUrl { get; set; }

		/// <summary>
		/// Advocate Created Date.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// A list of blocked brands linking details that this advocate is associated with.
		/// </summary>
		public List<AdvocateBlockedSearchModel> BlockHistory { get; set; }

		/// <summary>
		/// Determines the profile application status of an advocate.
		/// </summary>
		public string ProfileStatus { get; set; }
	}
}
