using System;
using MediatR;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CancelTicketReservationCommand : Command<Unit>, IScheduledJob
	{
		public string JobId => $"{nameof(CancelTicketReservationCommand)}-{TicketId}";
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		public Guid? AdvocateId { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public CancelTicketReservationCommand(Guid ticketId, Guid? advocateId)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
