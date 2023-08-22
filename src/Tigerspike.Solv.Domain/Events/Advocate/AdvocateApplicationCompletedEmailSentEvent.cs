using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationCompletedEmailSentEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationCompletedEmailSentEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}
