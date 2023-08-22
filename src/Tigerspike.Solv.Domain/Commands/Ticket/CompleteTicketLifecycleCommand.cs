using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CompleteTicketLifecycleCommand : Command<bool>
	{
		public Guid TicketId { get; set; }

		public string AdvocateEmail { get; set; }

		public string AdvocateAlternateEmail { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime ReservedDate { get; set; }

		public DateTime AssignedDate { get; set; }

		public DateTime SolvedDate { get; set; }

		public DateTime ClosedDate { get; set; }

		public int? Complexity { get; set; }

		public int? Csat { get; set; }

		public CompleteTicketLifecycleCommand(Guid ticketId, string advocateEmail, string advocateAlternateEmail,
			DateTime createdDate, DateTime reservedDate, DateTime assignedDate, DateTime solvedDate,
			DateTime closedDate, int? complexity, int? csat)
		{
			TicketId = ticketId;
			AdvocateEmail = advocateEmail;
			AdvocateAlternateEmail = advocateAlternateEmail;
			CreatedDate = createdDate;
			ReservedDate = reservedDate;
			AssignedDate = assignedDate;
			SolvedDate = solvedDate;
			ClosedDate = closedDate;
			Complexity = complexity;
			Csat = csat;
		}

		public override bool IsValid()
		{
			return TicketId != Guid.Empty;
		}
	}
}