using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationDeletedEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationDeletedEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}
