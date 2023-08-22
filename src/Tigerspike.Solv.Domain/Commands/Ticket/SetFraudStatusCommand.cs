using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetFraudStatusCommand : Command<Unit>
	{
		/// <summary>
		/// The identifier for list of tickets.
		/// </summary>
		public IEnumerable<Guid> TicketIds { get; }

		/// <summary>
		/// The new fraud status.
		/// </summary>
		public FraudStatus FraudStatus { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetFraudStatusCommand(FraudStatus fraudStatus, IEnumerable<Guid> ticketIds)
		{
			FraudStatus = fraudStatus;
			TicketIds = ticketIds;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketIds.ToList().Count > 0;
	}
}