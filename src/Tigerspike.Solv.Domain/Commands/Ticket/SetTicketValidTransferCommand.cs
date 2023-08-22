using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketValidTransferCommand : Command<bool>
	{
		/// <summary>
		/// Gets or sets ticket identifier.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Determines if or not escalated ticket was valid transfer by a regular solver.
		/// </summary>
		public bool IsValidTransfer { get; set; }

		/// <summary>
		/// Parameterised contructor.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="validTransfer">Valid transfer or not.</param>
		public SetTicketValidTransferCommand(Guid ticketId, bool isValidTransfer) => (TicketId, IsValidTransfer) = (ticketId, isValidTransfer);

		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
