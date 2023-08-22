using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketCsatSetEvent : Event
	{
		public Guid TicketId { get; }
		public Guid AdvocateId { get; }
		public Guid BrandId { get; set; }
		public string BrandName { get; set; }
		public bool BrandNpsEnabled { get; set; }
		public int Csat { get; }
		public bool IsPractice { get; set; }
		public TicketTransportType TransportType { get; }

		public TicketCsatSetEvent(Guid ticketId, Guid advocateId, Guid brandId, string brandName, bool isPractice, int csat, TicketTransportType transportType)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
			BrandId = brandId;
			BrandName = brandName;
			IsPractice = isPractice;
			Csat = csat;
			TransportType = transportType;
		}
	}
}