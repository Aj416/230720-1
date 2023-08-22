using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketAcceptedNotifiedEvent : Event
	{
		public Guid TicketId { get; }
		public Guid BrandId { get; }
		public Guid AdvocateId { get; }
		public string AdvocateFirstName { get; }
		public bool IsSuperSolver { get; }
		public TicketLevel Level { get; }
		public TicketEscalationReason? EscalationReason { get; }
		public DateTime FirstAssignedDate { get; set; }
		public DateTime AssignedDate { get; set; }
		public TicketAcceptedNotifiedEvent(Guid ticketId, Guid brandId, Guid advocateId, string advocateFirstName, bool isSuperSolver, TicketLevel level, TicketEscalationReason? escalationReason, DateTime firstAssignedDate, DateTime assignedDate)
		{
			TicketId = ticketId;
			BrandId = brandId;
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			IsSuperSolver = isSuperSolver;
			Level = level;
			EscalationReason = escalationReason;
			FirstAssignedDate = firstAssignedDate;
			AssignedDate = assignedDate;
		}
	}
}