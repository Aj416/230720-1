using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represents a line item of an advocate invoice.
	/// It holds information about a specific brand in an invoice.
	/// </summary>
	public class AdvocateInvoiceLineItem
	{
		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private AdvocateInvoiceLineItem() { }

		public AdvocateInvoiceLineItem(Guid brandId, decimal amount, int ticketsCount)
		{
			BrandId = brandId;
			Amount = amount;
			TicketsCount = ticketsCount;
		}

		public Guid Id { get; set; }

		/// <summary>
		/// The amount of all tickets for this brand in the parent invoice.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// The parent advocate invoice
		/// </summary>
		public AdvocateInvoice AdvocateInvoice { get; set; }

		/// <summary>
		/// The parent advocate invoice id.
		/// </summary>
		public Guid AdvocateInvoiceId { get; set; }

		/// <summary>
		/// The brand that this line item invoices.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The brand that this line item invoices.
		/// </summary>
		public Brand Brand { get; set; }

		/// <summary>
		/// The number of tickets for one brand to be paid by this line item.
		/// </summary>
		public int TicketsCount { get; set; }

		/// <summary>
		/// Payment execution confirmation record
		/// </summary>
		public Payment Payment { get; set; }
	}
}