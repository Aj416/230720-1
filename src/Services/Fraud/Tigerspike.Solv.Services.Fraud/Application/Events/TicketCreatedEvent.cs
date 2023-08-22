using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Fraud.Application.Events
{
	public class TicketCreatedEvent : Event
	{
		public Guid TicketId { get; }

		public TicketCreatedEvent(Guid ticketId) => TicketId = ticketId;
	}
}