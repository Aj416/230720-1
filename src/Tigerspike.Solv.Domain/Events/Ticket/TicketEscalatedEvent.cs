using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketEscalatedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid? AdvocateId { get; }
		public string AdvocateFirstName { get; set; }
		public decimal? AdvocateCsat { get; set; }
		public Guid BrandId { get; }
		public string ReferenceId { get; }

		public bool IsPractice { get; }

		public string Source { get; }

		public TicketStatusEnum FromStatus { get; }

		public TicketEscalationReason EscalationReason { get; }

		public TicketLevel Level { get; }

		public string ThreadId { get; }

		public Guid CustomerId { get; set; }

		public TicketEscalatedEvent(Guid ticketId, Guid? advocateId, TicketLevel level, string advocateFirstName,
			decimal? advocateCsat, Guid brandId, string referenceId, string threadId, bool isPractice,
			string source, TicketStatusEnum fromStatus, TicketEscalationReason escalationReason, Guid customerId)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
			Level = level;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
			BrandId = brandId;
			ReferenceId = referenceId;
			ThreadId = threadId;
			IsPractice = isPractice;
			Source = source;
			FromStatus = fromStatus;
			EscalationReason = escalationReason;
			CustomerId = customerId;
		}
	}
}