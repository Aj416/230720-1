using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	/// <summary>
	/// Data to generate printable document for client invoice
	/// </summary>
	public class ClientInvoicePrintableModel
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
		/// Total ticket price of the tickets on the invoice
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Fee amount on the invoice
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// Subtotal amount on the invoice (before taxes)
		/// </summary>
		public decimal Subtotal { get; set; }

		/// <summary>
		/// VAT rate for invoice
		/// </summary>
		public decimal? VatRate { get; set; }

		/// <summary>
		/// VAT amount for invoice
		/// </summary>
		public decimal? VatAmount { get; set; }

		/// <summary>
		/// Total amount on the invoice (after taxes)
		/// </summary>
		public decimal InvoiceTotal { get; set; }

		/// <summary>
		/// Total amount on the payment advice (after taxes)
		/// </summary>
		public decimal PaymentTotal { get; set; }

		/// <summary>
		/// Invoice reference number
		/// </summary>
		public string ReferenceNumber { get; set; }

		/// <summary>
		/// Brand payment route name
		/// </summary>
		public string PaymentRouteName { get; set; }

		/// <summary>
		/// Invoicing cycle details
		/// </summary>
		public InvoicingCyclePrintableModel InvoicingCycle { get; set; }

		/// <summary>
		/// Brand details
		/// </summary>
		public BillingDetailsPrintableModel BrandBillingDetails { get; set; }

		/// <summary>
		/// Details of subject that issued the invoice
		/// </summary>
		public BillingDetailsPrintableModel PlatformBillingDetails { get; set; }

		/// <summary>
		/// Issue date of the invoice
		/// </summary>
		public IEnumerable<TicketPrintableModel> Tickets { get; set; }
	}
}
