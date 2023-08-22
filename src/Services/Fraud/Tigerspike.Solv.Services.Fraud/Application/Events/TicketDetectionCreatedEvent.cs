using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Fraud.Application.Events
{
	public class TicketDetectionCreatedEvent : Event
	{
		public Guid TicketId { get; }

		public TicketDetectionCreatedEvent(Guid ticketId) => TicketId = ticketId;
	}
}