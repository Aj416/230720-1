using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketChaserEmailSentEvent : Event
	{
		public Guid TicketId { get; }

		public TicketChaserEmailSentEvent(Guid ticketId)
		{
			TicketId = ticketId;
		}
	}
}