using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketCsatCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The Csat value.
		/// </summary>
		public int Csat { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketCsatCommand(Guid ticketId, int csat)
		{
			TicketId = ticketId;
			Csat = csat;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		 Csat >= Models.Ticket.MIN_CSAT &&
		 Csat <= Models.Ticket.MAX_CSAT;
	}
}