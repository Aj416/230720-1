using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// Reserve a ticket for a user so he/she can choose to accept/reject it before it is assigned to
	/// him.
	/// </summary>
	public class ReserveTicketCommand : Command<Guid?>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReserveTicketCommand(Guid advocateId) => AdvocateId = advocateId;

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