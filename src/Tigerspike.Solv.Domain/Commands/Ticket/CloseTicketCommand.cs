using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CloseTicketCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// Who asked to close the ticket.
		/// </summary>
		public ClosedBy ClosedBy { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public CloseTicketCommand(Guid ticketId, ClosedBy closedBy) => (TicketId, ClosedBy) = (ticketId, closedBy);

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}