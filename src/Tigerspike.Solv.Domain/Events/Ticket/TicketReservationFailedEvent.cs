using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketReservationFailedEvent : Event
	{
		public Guid AdvocateId { get; }
		public Guid[] BrandIds { get; }
		public TicketLevel Level { get;}

		public TicketReservationFailedEvent(Guid advocateId, Guid[] brandIds, TicketLevel level)
		{
			AdvocateId = advocateId;
			BrandIds = brandIds;
			Level = level;
		}
	}
}