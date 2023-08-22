using System;

namespace Tigerspike.Solv.Application.Models.Statistics
{
	public class TicketStatisticsForBillingCycleModel
	{
		public int TicketCount { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime IssueDate { get; set; }
	}
}