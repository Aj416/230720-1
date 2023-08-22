using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketPendingStatusUpdatedEvent : Event
	{
		/// <summary>
		/// The ticket id.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The brand id.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The advocate of this ticket.
		/// Can be null if the ticket has just been acceptted for instance.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The ticket status
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		public TicketPendingStatusUpdatedEvent(Guid ticketId, Guid? advocateId, Guid brandId, TicketStatusEnum status)
		=> (TicketId, AdvocateId, BrandId, Status) = (ticketId, advocateId, brandId, status);
	}
}
