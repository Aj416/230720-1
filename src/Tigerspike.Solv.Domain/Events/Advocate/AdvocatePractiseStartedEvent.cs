using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocatePractiseStartedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocatePractiseStartedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}