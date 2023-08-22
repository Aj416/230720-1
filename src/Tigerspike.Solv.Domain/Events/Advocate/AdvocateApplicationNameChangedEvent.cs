using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationNameChangedEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationNameChangedEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}