using System;
using MediatR;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class StartTicketTimeoutEscalation : Command<Unit>, IScheduledJob
	{
		public string JobId => $"{nameof(StartTicketTimeoutEscalation)}-{TicketId}";

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public StartTicketTimeoutEscalation(Guid ticketId) => TicketId = ticketId;

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
