using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CompleteTicketCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// Who asked to close the ticket.
		/// </summary>
		public TicketTagStatus TagStatus { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public CompleteTicketCommand(Guid ticketId, TicketTagStatus tagStatus) => (TicketId, TagStatus) = (ticketId, tagStatus);

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}