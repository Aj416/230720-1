using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketComplexityCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The ticket complexity value.
		/// </summary>
		public int Complexity { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketComplexityCommand(Guid ticketId, int complexity)
		{
			TicketId = ticketId;
			Complexity = complexity;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
			TicketId != Guid.Empty &&
			Complexity >= Models.Ticket.MIN_COMPLEXITY &&
			Complexity <= Models.Ticket.MAX_COMPLEXITY;
	}
}