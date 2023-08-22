using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketNpsCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The NPS value.
		/// </summary>
		public int Nps { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketNpsCommand(Guid ticketId, int nps)
		{
			TicketId = ticketId;
			Nps = nps;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		Nps >= Models.Ticket.MIN_NPS &&
		Nps <= Models.Ticket.MAX_NPS;
	}
}