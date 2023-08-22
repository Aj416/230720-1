using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketImportedEvent : Event
	{
		public Guid TicketId { get; }
		public string ReferenceId { get; }
		public Guid BrandId { get; }
		public Guid? AdvocateId { get; set; }
		public DateTime? ClosedDate { get; set; }
		public int? Csat { get; set; }

		public TicketImportedEvent(Guid ticketId, string referenceId, Guid brandId, Guid? advocateId, DateTime? closedDate, int? csat)
		{
			TicketId = ticketId;
			ReferenceId = referenceId;
			BrandId = brandId;
			AdvocateId = advocateId;
			ClosedDate = closedDate;
			Csat = csat;
		}
	}
}