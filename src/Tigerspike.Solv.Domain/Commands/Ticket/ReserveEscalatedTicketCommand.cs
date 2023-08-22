using System;
using System.Collections.Generic;
using System.Text;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// Reserve an escalated ticket (mainly for super solver)
	/// </summary>
	public class ReserveEscalatedTicketCommand : Command<Guid?>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReserveEscalatedTicketCommand(Guid advocateId) => AdvocateId = advocateId;

		/// <summary>
		/// The advocate id to reserve a ticket for.
		/// </summary>
		public Guid AdvocateId { get; }

		/// <summary>
		/// Returns if the command is valid
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}
