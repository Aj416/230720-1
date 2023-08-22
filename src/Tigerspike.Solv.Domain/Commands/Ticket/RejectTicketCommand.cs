using System;
using System.Linq;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class RejectTicketCommand : Command<Unit>
	{
		public Guid TicketId { get; }

		public int[] RejectReasonIds { get; }

		public RejectTicketCommand(Guid ticketId, int[] rejectReasonIds)
		{
			TicketId = ticketId;
			RejectReasonIds = rejectReasonIds;
		}

		public override bool IsValid() => TicketId != Guid.Empty && RejectReasonIds.Any();
	}
}
