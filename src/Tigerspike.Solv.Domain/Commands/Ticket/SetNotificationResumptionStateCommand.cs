using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetNotificationResumptionStateCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }
		public NotificationResumptionState State { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
	public SetNotificationResumptionStateCommand(Guid ticketId, NotificationResumptionState notificationResumptionState)
		{
			TicketId = ticketId;
			State = notificationResumptionState;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketId != Guid.Empty && State != NotificationResumptionState.NotStarted;
	}
}
