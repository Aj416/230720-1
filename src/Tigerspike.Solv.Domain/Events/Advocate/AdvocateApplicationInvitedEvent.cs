using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationInvitedEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationInvitedEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}