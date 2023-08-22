using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateCreatedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocateCreatedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}
