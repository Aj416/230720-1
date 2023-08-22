using System;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Invoice details for the client
	/// </summary>
	public class AdvocateInvoiceModel
	{
		/// <summary>
		/// The system identifier of the invoice
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// A friendly generated identifier of this invoice to be displayed.
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// Issue date of the invoice
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Total amount on the invoice
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// Advocate on the invoice
		/// </summary>
		public string AdvocateFullName { get; set; }

		public AdvocateStatus AdvocateStatus { get; set; }

		/// <summary>
		/// The invoice status
		/// </summary>
		public AdvocateInvoiceStatus Status { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime? PaymentFailureDate { get; set; }
	}
}
