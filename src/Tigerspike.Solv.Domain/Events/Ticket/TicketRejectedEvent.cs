using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketRejectedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public Guid AdvocateId { get; }

		public int? SourceId { get; }

		public DateTime CreatedDate { get; }

		public int RejectionCount { get; }

		public TicketLevel Level { get; }

		public Guid CustomerId { get; set; }

		public TicketRejectedEvent(Guid id, Guid brandId, TicketLevel level, Guid advocateId, int? sourceId, DateTime createdDate, int rejectionCount, Guid customerId)
		{
			TicketId = id;
			BrandId = brandId;
			Level = level;
			SourceId = sourceId;
			CreatedDate = createdDate;
			RejectionCount = rejectionCount;
			AdvocateId = advocateId;
			CustomerId = customerId;
		}
	}
}