using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class DeleteTicketCommand : Command<Unit>
	{
		public Guid TicketId { get; }

		public DeleteTicketCommand(Guid ticketId) => TicketId = ticketId;

		public override bool IsValid() => TicketId != Guid.Empty;
	}
}