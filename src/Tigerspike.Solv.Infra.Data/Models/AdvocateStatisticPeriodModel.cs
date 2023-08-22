using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class AdvocateStatisticPeriodModel
	{
		public int ClosedTickets { get; set; }
		public decimal Amount { get; set; }
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }

		/// <summary>
		/// The date of the last payment (if any) during the period specified in this model.
		/// </summary>
		public DateTime? PaidOn { get; set; }
	}
}