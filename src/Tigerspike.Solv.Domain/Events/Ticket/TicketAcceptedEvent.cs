using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketAcceptedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public string ReferenceId { get; }

		public bool IsPractice { get; }

		public Guid AdvocateId { get; }

		public string AdvocateFirstName { get; }

		public decimal AdvocateCsat { get; }

		public bool IsSuperSolver { get; }

		public string SourceName { get; }

		public TicketLevel Level { get; }
		public TicketEscalationReason? EscalationReason { get; }
		public TicketTransportType TransportType { get; set; }
		public DateTime FirstAssignedDate { get; set; }
		public DateTime AssignedDate { get; set; }
		public Guid CustomerId { get; set; }

		public TicketAcceptedEvent(Guid ticketId, Guid brandId, string referenceId, bool isPractice, Guid advocateId,
			string advocateFirstName, decimal advocateCsat, bool isSuperSolver, string sourceName, TicketLevel level,
			TicketEscalationReason? escalationReason, TicketTransportType transportType, DateTime firstAssignedDate, DateTime assignedDate, Guid customerId)
		{
			TicketId = ticketId;
			BrandId = brandId;
			ReferenceId = referenceId;
			IsPractice = isPractice;
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
			IsSuperSolver = isSuperSolver;
			SourceName = sourceName;
			Level = level;
			EscalationReason = escalationReason;
			TransportType = transportType;
			FirstAssignedDate = firstAssignedDate;
			AssignedDate = assignedDate;
			CustomerId = customerId;
		}
	}
}