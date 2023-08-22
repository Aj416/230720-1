using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	/// <summary>
	/// Data to generate printable document for advocate invoice
	/// </summary>
	public class AdvocateInvoicePrintableModel
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
		/// Total amount on the invoice
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// Invoice reference number
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// Invoicing cycle details
		/// </summary>
		public InvoicingCyclePrintableModel InvoicingCycle { get; set; }

		/// <summary>
		/// Advocate's name on the invoice
		/// </summary>
		public AdvocatePrintableModel Advocate { get; set; }

		/// <summary>
		/// Details of subject that issued the invoice
		/// </summary>
		public BillingDetailsPrintableModel PlatformBillingDetails { get; set; }

		/// <summary>
		/// Invoice lines (per brand specific informations)
		/// </summary>
		public IEnumerable<AdvocateInvoiceLineItemPrintableModel> LineItems { get; set; }

		/// <summary>
		/// Tickets account for in the invoice
		/// </summary>
		public IEnumerable<TicketPrintableModel> Tickets { get; set; }

		/// <summary>
		/// The date of the payment failure.
		/// </summary>
		public DateTime PaymentFailureDate { get; set; }
	}
}
