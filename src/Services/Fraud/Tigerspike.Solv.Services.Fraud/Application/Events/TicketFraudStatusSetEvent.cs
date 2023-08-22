using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Fraud.Application.Events
{
	public class TicketFraudStatusSetEvent : Event
	{
		public IEnumerable<Guid> TicketIds { get; }

		public TicketFraudStatusSetEvent(IEnumerable<Guid> ticketIds) => TicketIds = ticketIds;
	}
}