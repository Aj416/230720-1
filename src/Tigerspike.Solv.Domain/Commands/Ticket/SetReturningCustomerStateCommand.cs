using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetReturningCustomerStateCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }
		public ReturningCustomerState State { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
	public SetReturningCustomerStateCommand(Guid ticketId, ReturningCustomerState returningCustomerState)
		{
			TicketId = ticketId;
			State = returningCustomerState;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketId != Guid.Empty && State != ReturningCustomerState.NotStarted;
	}
}
