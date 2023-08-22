using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketReservedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public Guid AdvocateId { get; }

		public string AdvocateFirstName { get; }

		public decimal AdvocateCsat { get; }

		public DateTime ReservationExpiryDate { get; }

		public Guid CustomerId { get; set; }

		public TicketReservedEvent(Guid ticketId, Guid brandId, Guid advocateId, string advocateFirstName,
			decimal advocateCsat, DateTime reservationExpiryDate, Guid customerId)
		{
			TicketId = ticketId;
			BrandId = brandId;
			AdvocateId = advocateId;
			ReservationExpiryDate = reservationExpiryDate;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
			CustomerId = customerId;
		}
	}
}