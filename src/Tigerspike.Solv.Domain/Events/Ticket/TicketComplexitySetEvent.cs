using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketComplexitySetEvent : Event
	{
		public Guid? AdvocateId { get; }
		public Guid TicketId { get; }

		public bool IsPractice { get; }

		public TicketComplexitySetEvent(Guid ticketId, Guid? advocateId, bool isPractice = false)
		{
			TicketId = ticketId;
			IsPractice = isPractice;
			AdvocateId = advocateId;
		}
	}
}