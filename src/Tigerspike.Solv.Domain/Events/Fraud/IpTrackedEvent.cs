using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class IpTrackedEvent : Event
	{
		public Guid TicketId { get; }
		public string IpAddress { get; }
		public string EventName { get; }

		public IpTrackedEvent(Guid ticketId, string ipAddress, string eventName)
		{
			TicketId = ticketId;
			IpAddress = ipAddress;
			EventName = eventName;
		}
	}
}