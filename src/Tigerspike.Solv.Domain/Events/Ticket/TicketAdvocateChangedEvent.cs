using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	/// <summary>
	/// Indicates that the current advocate of a ticket has changed
	/// for whatever reason.
	/// </summary>
	public class TicketAdvocateChangedEvent : Event
	{
		public TicketAdvocateChangedEvent(Guid ticketId, TicketStatusEnum ticketStatus, Guid? oldAdvocateId,
			Guid? newAdvocateId, string newAdvocateFirstName, decimal? newAdvocateCsat)
			=> (TicketId, TicketStatus, OldAdvocateId, NewAdvocateId, NewAdvocateFirstName, NewAdvocateCsat)
			= (ticketId, ticketStatus, oldAdvocateId, newAdvocateId, newAdvocateFirstName, newAdvocateCsat);

		/// <summary>
		/// The ticket id.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The ticket status.
		/// </summary>
		public TicketStatusEnum TicketStatus { get; set; }

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
