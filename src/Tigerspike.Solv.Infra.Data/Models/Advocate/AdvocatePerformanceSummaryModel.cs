using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Advocate Performance Summary Model.
	/// </summary>
	public class AdvocatePerformanceSummaryModel
	{
		/// <summary>
		/// List of Brand ClosedTicketSummaryModel.
		/// </summary>
		public IList<BrandClosedTicketSummaryModel> Brands { get; set; }

		/// <summary>
		/// Closed Ticket TotalSummary Model.
		/// </summary>
		public ClosedTicketTotalSummaryModel Total { get; set; }
	}
}