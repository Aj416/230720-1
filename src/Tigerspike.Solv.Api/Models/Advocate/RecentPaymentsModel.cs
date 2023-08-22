using System.Collections.Generic;

namespace Tigerspike.Solv.Api.Models.Advocate
{
	/// <summary>
	/// Payment details for the advocate
	/// </summary>
    public class RecentPaymentsModel
    {
		/// <summary>
		/// Total count of the payments for the advocate
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Url to external system handling the payments execution
		/// </summary>
		public string ExternalPaymentsUrl { get; set; }

		/// <summary>
		/// List of recent payments
		/// </summary>
		public IEnumerable<PaymentModel> RecentList { get; set; }
    }
}