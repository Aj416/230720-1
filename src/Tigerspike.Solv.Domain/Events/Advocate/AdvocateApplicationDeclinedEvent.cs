using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationDeclinedEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationDeclinedEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}
