using System;

namespace Tigerspike.Solv.Messaging.Ticket
{
	public interface ITicketAdvocateChangedEvent
	{
		/// <summary>
		/// The ticket id.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The ticket status.
		/// </summary>
		public int TicketStatus { get; set; }

		/// <summary>
		/// The previous advocate of this ticket.
		/// Can be null if the ticket has just been acceptted for instance.
		/// </summary>
		public Guid? OldAdvocateId { get; set; }

		/// <summary>
		/// The new advocate of this ticket.
		/// Can be null if the ticket is abandoned for instance.
		/// </summary>
		public Guid? NewAdvocateId { get; set; }

		/// <summary>
		/// The new advocate first name.
		/// </summary>
		public string NewAdvocateFirstName { get; set; }

		/// <summary>
		/// The new advocate csat.
		/// </summary>
		public decimal? NewAdvocateCsat { get; set; }
	}
}