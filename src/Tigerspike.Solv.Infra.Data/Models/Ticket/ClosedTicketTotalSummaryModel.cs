namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Closed Ticket Total Summary Model.
	/// </summary>
	public class ClosedTicketTotalSummaryModel
	{
		/// <summary>
		/// The closed ticket count for a given period.
		/// </summary>
		public int ClosedTickets { get; set; }

		/// <summary>
		/// The total price of tickets closed for a given period.
		/// </summary>
		public decimal Amount { get; set; }
	}
}