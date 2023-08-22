using System;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Ticket printable model
	/// </summary>
	public class TicketPrintableModel
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The brand id of that ticket is for
		/// </summary>
		public BrandPrintableModel Brand { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		public decimal Fee { get; set; }

		/// <summary>
		/// The price of this ticket.
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// The creation date of the ticket.
		/// </summary>
		public DateTime CreatedDate { get; set; }
	}
}