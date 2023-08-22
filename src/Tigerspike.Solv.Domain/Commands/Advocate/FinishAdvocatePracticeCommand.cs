using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class FinishAdvocatePracticeCommand : Command<Unit>
	{
		/// <summary>
		/// The practice ticket id that the advocate has finished.
		/// </summary>
		public Guid TicketId { get; }

		public FinishAdvocatePracticeCommand(Guid ticketId) => TicketId = ticketId;

		public override bool IsValid() => TicketId != Guid.Empty;
	}
}