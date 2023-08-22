using System;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Ticket Export Request Model
	/// </summary>
	public class TicketExportRequestModel
	{
		/// <summary>
		/// From timestamp
		/// </summary>
		public DateTime? From { get; set; }

		/// <summary>
		/// To timestamp
		/// </summary>
		public DateTime? To { get; set; }


		/// <summary>
		/// Filters the tickets by the brand specified.
		/// </summary>
		public Guid? BrandId { get; set; }
	}
}