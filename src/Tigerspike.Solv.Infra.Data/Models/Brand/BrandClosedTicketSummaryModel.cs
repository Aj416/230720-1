using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Brand Closed Ticket Summary Model.
	/// </summary>
	public class BrandClosedTicketSummaryModel
	{
		/// <summary>
		/// The brand identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The total price of tickets closed for a given period.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// The closed ticket count with respect to a brand for a given period.
		/// </summary>
		public int ClosedTickets { get; set; }
	}
}