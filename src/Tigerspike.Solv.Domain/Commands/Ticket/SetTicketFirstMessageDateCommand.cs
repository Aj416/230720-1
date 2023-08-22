using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketFirstMessageDateCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The message created date
		/// </summary>
		public DateTime CreatedDate { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketFirstMessageDateCommand(Guid ticketId, DateTime createdDate)
		{
			TicketId = ticketId;
			CreatedDate = createdDate;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty && CreatedDate != DateTime.MinValue;

	}
}