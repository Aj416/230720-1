using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// Reserve and accept ticket.
	/// </summary>
	public class ReserveAndAcceptTicketCommand : Command<Guid?>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReserveAndAcceptTicketCommand(Guid advocateId) => AdvocateId = advocateId;

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