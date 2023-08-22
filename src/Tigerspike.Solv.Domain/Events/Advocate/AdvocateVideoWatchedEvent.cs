using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateVideoWatchedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocateVideoWatchedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}