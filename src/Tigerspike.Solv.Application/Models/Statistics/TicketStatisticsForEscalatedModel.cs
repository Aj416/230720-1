using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Statistics
{
	public class TicketStatisticsForEscalatedModel
	{

		public IList<TicketStatisticsSourceCount> Items { get; set; }
		public int Total { get; set; }
	}
}