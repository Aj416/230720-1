using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketRatingEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public TicketTransportType TransportType { get; }

		public Guid? AdvocateId { get; }

		public TicketRatingEvent(Guid ticketId, Guid brandId, TicketTransportType transportType, Guid? advocateId)
		{
			TicketId = ticketId;
			BrandId = brandId;
			TransportType = transportType;
			AdvocateId = advocateId;
		}
	}
}