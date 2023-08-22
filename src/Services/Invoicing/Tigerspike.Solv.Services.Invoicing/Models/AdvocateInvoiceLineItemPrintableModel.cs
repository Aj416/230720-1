using System;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class AdvocateInvoiceLineItemPrintableModel
	{
		/// <summary>
		/// The brand name that invoice line relates to
		/// </summary>
		public string Brand { get; set; }

		/// <summary>
		/// The brand identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Amount of the line
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// Tickets count of the line
		/// </summary>
		public int TicketsCount { get; set; }
	}
}
