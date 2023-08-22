using System;

namespace Tigerspike.Solv.Messaging.Fraud
{
	/// <summary>
	/// Serial number info for past week.
	/// </summary>
	public interface ISerialNumber
	{
		/// <summary>
		/// Ticket created date.
		/// </summary>
		DateTime CreatedDate { get; set; }

		/// <summary>
		/// Ticket Serial number.
		/// </summary>
		string Serial { get; set; }

		/// <summary>
		/// Customer id.
		/// </summary>
		Guid CustomerId { get; set; }

		/// <summary>
		/// Solver id.
		/// </summary>
		Guid? SolverId { get; set; }

		/// <summary>
		/// Ticket id.
		/// </summary>
		Guid TicketId { get; set; }
	}
}