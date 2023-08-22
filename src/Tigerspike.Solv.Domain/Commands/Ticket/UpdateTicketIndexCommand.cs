using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// The command to update the ticket index
	/// </summary>
	public class UpdateTicketIndexCommand : Command<Unit>
	{
		/// <summary>
		/// The TicketId
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
