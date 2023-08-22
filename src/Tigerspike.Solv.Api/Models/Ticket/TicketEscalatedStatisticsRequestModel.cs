using System;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Ticket Statistics Export Request Model
	/// </summary>
	public class TicketStatisticsRequestModel
	{
		/// <summary>
		/// From timestamp
		/// </summary>
		public DateTime? From { get; set; }

		/// <summary>
		/// To timestamp
		/// </summary>
		public DateTime? To { get; set; }
	}
}