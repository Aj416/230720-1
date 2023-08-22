using System;

namespace Tigerspike.Solv.Api.Models.Advocate
{
	/// <summary>
	/// Payment details for the advocate
	/// </summary>
    public class PaymentModel
    {
		/// <summary>
		/// The system identifier of the payment
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Issue date of the payment
		/// </summary>
		public DateTime IssueDate { get; set; }

		/// <summary>
		/// Total amount of the payment
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// Url to download the payment document
		/// </summary>
		public string DocumentUrl { get; set; }
    }
}