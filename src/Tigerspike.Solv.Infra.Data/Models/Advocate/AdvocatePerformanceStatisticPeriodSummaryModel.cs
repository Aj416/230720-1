using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models
{
	/// <summary>
	/// Advocate Performance Statistic Period Summary Model.
	/// </summary>
	public class AdvocatePerformanceStatisticPeriodSummaryModel
	{
		/// <summary>
		/// List of AdvocatePerformanceBreakdownModel.
		/// </summary>		
		public IList<AdvocatePerformanceBreakDownModel> Breakdown { get; set; }

		/// <summary>
		/// Advocate Performance Summary Model.
		/// </summary>
		public AdvocatePerformanceSummaryModel Summary { get; set; }
	}
}