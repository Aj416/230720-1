namespace Tigerspike.Solv.Application.Models.Statistics
{
	public class TicketStatisticsOverviewModel
	{

		public double AverageResponseTime { get; set; }

		public double AverageTimeToComplete { get; set; }

		public decimal AveragePrice { get; set; }

		public decimal AverageComplexity { get; set; }

		public int ClosedFirstRound { get; set; }

		public int SuccessRate { get; set; }

		public decimal AverageCsat { get; set; }

		public decimal TotalPrice { get; set; }
	}
}