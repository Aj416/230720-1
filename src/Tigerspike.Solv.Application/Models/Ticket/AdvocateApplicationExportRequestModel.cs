using System;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Ticket Export Request Model
	/// </summary>
	public class AdvocateApplicationExportRequestModel
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