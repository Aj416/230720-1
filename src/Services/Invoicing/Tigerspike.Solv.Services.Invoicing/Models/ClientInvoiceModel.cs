using System;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	/// <summary>
	/// Invoice details for the client
	/// </summary>
	public class ClientInvoiceModel
	{
		/// <summary>
		/// The system identifier of the invoice
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Issue date of the invoice
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Number of tickets accounted for
		/// </summary>
		public int TicketsCount { get; set; }

		/// <summary>
		/// Subtotal amount on the invoice (before taxes)
		/// </summary>
		public decimal Subtotal { get; set; }

		/// <summary>
		/// Invoice reference number
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime PaymentFailureDate { get; set; }
	}
}
