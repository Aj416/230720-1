using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketSposLeadSetEvent : Event
	{
		public Guid TicketId { get; }
		public Guid CustomerId { get; }
		public Guid BrandId { get; }
		public bool Notify { get; }

		public TicketSposLeadSetEvent(Guid ticketId, Guid customerId, Guid brandId, bool notify)
		{
			TicketId = ticketId;
			CustomerId = customerId;
			BrandId = brandId;
			Notify = notify;
		}
	}
}