using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketNpsSetEvent : Event
	{
		public Guid TicketId { get; }

		public Guid AdvocateId { get; }

		public Guid BrandId { get; set; }

		public int Nps { get; }

		public bool IsPractice { get; set; }

		public TicketTransportType TransportType { get; }

		public TicketNpsSetEvent(Guid ticketId, Guid advocateId, Guid brandId, bool isPractice, int nps,
			TicketTransportType transportType)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
			BrandId = brandId;
			IsPractice = isPractice;
			Nps = nps;
			TransportType = transportType;
		}
	}
}