
namespace Tigerspike.Solv.Application.Models.Statistics
{
	public class TicketStatisticsSourceCount
	{

		public string Source { get; set; }
		public int Count { get; set; }

		public TicketStatisticsSourceCount(string source, int count)
		{
			Source = source;
			Count = count;
		}
	}
}