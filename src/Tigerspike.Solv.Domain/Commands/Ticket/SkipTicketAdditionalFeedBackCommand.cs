using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SkipTicketAdditionalFeedBackCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// Paramterised constructor.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		public SkipTicketAdditionalFeedBackCommand(Guid ticketId)
		=> TicketId = ticketId;

		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
