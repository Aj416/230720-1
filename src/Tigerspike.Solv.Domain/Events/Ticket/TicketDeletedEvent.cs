using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketDeletedEvent : Event
	{
		public Guid TicketId { get; }

		public bool IsPractice { get; }

		public TicketDeletedEvent(Guid id, bool isPractice)
		{
			TicketId = id;
			IsPractice = isPractice;
		}
	}
}