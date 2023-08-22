using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketFraudSetEvent : Event
	{
		public Guid TicketId { get; }
		public Guid? AdvocateId { get; }
		public Guid BrandId { get; set; }
		public FraudStatus FraudStatus { get; }

		public TicketFraudSetEvent(Guid ticketId, Guid? advocateId, Guid brandId, FraudStatus fraudStatus)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
			BrandId = brandId;
			FraudStatus = fraudStatus;
		}
	}
}