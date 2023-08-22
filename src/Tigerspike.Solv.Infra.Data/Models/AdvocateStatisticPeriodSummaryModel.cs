using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class AdvocateStatisticPeriodSummaryModel
	{
		public AdvocateStatisticPeriodModel PreviousWeek { get; set; }
		public AdvocateStatisticPeriodModel CurrentWeek { get; set; }
		public AdvocateStatisticPeriodModel AllTime { get; set; }
	}
}