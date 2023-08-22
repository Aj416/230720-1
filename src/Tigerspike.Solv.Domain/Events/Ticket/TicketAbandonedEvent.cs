using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketAbandonedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public string ReferenceId { get; }

		public bool IsPractice { get; }

		public Guid AdvocateId { get; }

		public int? SourceId { get; }

		public string SourceName { get; }

		public DateTime CreatedDate { get; }

		public int AbandonedCount { get; }

		public string AdvocateFirstName { get; }
		public TicketLevel Level { get; }
		public bool AutoAbandoned { get; }
		public TicketFlowAction? Action { get; }
		public TimeSpan NewStatusDuration { get; }
		public Guid CustomerId { get; set; }

		public TicketAbandonedEvent(Guid id, Guid brandId, TicketLevel level, string referenceId, bool isPractice, Guid advocateId, int? sourceId, string sourceName,
			DateTime createdDate, int abandonedCount, string advocateFirstName, bool autoAbandoned, TicketFlowAction? action, TimeSpan newStatusDuration, Guid customerId)
		{
			TicketId = id;
			BrandId = brandId;
			Level = level;
			ReferenceId = referenceId;
			IsPractice = isPractice;
			SourceId = sourceId;
			SourceName = sourceName;
			CreatedDate = createdDate;
			AbandonedCount = abandonedCount;
			AdvocateFirstName = advocateFirstName;
			AdvocateId = advocateId;
			AutoAbandoned = autoAbandoned;
			Action = action;
			NewStatusDuration = newStatusDuration;
			CustomerId = customerId;
		}
	}
}