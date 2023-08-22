using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Search
{
	/// <summary>
	/// The advocate search model that is returned by search API.
	/// </summary>
	public class AdvocateSearchResponseModel
	{
		public Guid Id { get; set; }

		public string FullName { get; set; }

		public decimal Csat { get; set; }

		/// <summary>
		/// The status of the advocate.
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Represent the status of this advocate for a specific brand.
		/// This will be filled only at the time of searching and if the results are filtered by a brand.
		/// </summary>
		public string BrandStatus { get; set; }

		/// <summary>
		/// Indicates that the user has setup the payment method.
		/// </summary>
		public bool PaymentMethodSetup { get; set; }

		/// <summary>
		/// Indicates that the advocate has verified his payment account email.
		/// </summary>
		public bool PaymentEmailVerified { get; set; }

		/// <summary>
		/// Indicates whether the advocate watched the video.
		/// </summary>
		public bool VideoWatched { get; set; }

		/// <summary>
		/// Indicates whether the advocate has completed practice.
		/// </summary>
		public bool PracticeComplete { get; set; }

		/// <summary>
		/// Identity verification status
		/// </summary>
		public IdentityVerificationStatus IdentityVerificationStatus { get; set; }

		/// <summary>
		/// Checks if the status of the identity verification is completed.
		/// </summary>
		public bool IsIdentityVerified => IdentityVerificationStatus == IdentityVerificationStatus.Completed;

		/// <summary>
		/// List of brands yet to be authorised.
		/// </summary>
		public IEnumerable<string> UnAuthorisedBrandNames { get; set; }

		/// <summary>
		/// List of invited status for unauthorised brands.
		/// </summary>
		public IEnumerable<string> InvitedStatus { get; set; }	

		public IEnumerable<string> BlockBrandNames { get; set; }	
	}
}