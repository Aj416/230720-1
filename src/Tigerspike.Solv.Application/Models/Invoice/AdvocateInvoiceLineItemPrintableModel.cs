
namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Advocate invoice line item (for printing)
	/// </summary>
	public class AdvocateInvoiceLineItemPrintableModel
	{
		/// <summary>
		/// The brand name that invoice line relates to
		/// </summary>
		public string Brand { get; set; }

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