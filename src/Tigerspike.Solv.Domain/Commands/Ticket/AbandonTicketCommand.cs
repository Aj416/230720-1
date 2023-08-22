using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class AbandonTicketCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// Array of abandon reasons
		/// </summary>
		public Guid[] AbandonReasonIds { get; }

		public bool AutoAbandoned { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public AbandonTicketCommand(Guid ticketId, Guid[] abandonReasonIds)
		{
			TicketId = ticketId;
			AbandonReasonIds = abandonReasonIds;
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public AbandonTicketCommand(Guid ticketId, bool autoAbandoned)
		{
			TicketId = ticketId;
			AutoAbandoned = autoAbandoned;
			AbandonReasonIds = new Guid[] { };
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
