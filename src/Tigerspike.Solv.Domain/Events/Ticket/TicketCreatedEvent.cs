using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketCreatedEvent : Event
	{
		public Guid TicketId { get; }

		public string ReferenceId { get; }

		public Guid BrandId { get; }

		public Guid CustomerId { get; set; }

		public string Question { get; }

		public int? SourceId { get; }

		public string SourceName { get; }

		public TicketTransportType TransportType { get; set; }

		public bool IsPractice { get; }

		public string ThreadId { get; set; }

		public TicketLevel Level { get; set; }
		public string Culture { get; }

		public TicketCreatedEvent(Guid id, string referenceId, string threadId, Guid brandId, Guid customerId, string question,
			int? sourceId, string sourceName,
			TicketTransportType transportType, bool isPractice, TicketLevel level, string culture)
		{
			TicketId = id;
			ReferenceId = referenceId;
			ThreadId = threadId;
			BrandId = brandId;
			CustomerId = customerId;
			Question = question;
			SourceId = sourceId;
			SourceName = sourceName;
			TransportType = transportType;
			IsPractice = isPractice;
			Level = level;
			Culture = culture;
		}
	}
}