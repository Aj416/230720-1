using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class ExpireTicketReservationCommand : Command<Domain.Models.Ticket>
	{
		public Guid TicketId { get; set; }

		public ExpireTicketReservationCommand(Guid ticketId) => TicketId = ticketId;

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}