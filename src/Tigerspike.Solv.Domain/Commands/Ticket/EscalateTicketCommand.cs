using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class EscalateTicketCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; protected set; }

		/// <summary>
		/// Escalation reason
		/// </summary>
		public TicketEscalationReason Reason { get; protected set; }

		/// <summary>
		/// Level to which escelate the ticket
		/// </summary>
		public TicketLevel? Level { get; protected set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected EscalateTicketCommand()
		{

		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public EscalateTicketCommand(Guid ticketId, TicketEscalationReason reason, TicketLevel? level = null)
		{
			TicketId = ticketId;
			Reason = reason;
			Level = level;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketId != Guid.Empty && Level != TicketLevel.Regular;
	}
}