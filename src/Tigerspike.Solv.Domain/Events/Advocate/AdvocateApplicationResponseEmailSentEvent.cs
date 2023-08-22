using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationResponseEmailSentEvent : Event
	{
		public Guid ApplicationId { get; }

		public AdvocateApplicationResponseEmailSentEvent(Guid applicationId) => ApplicationId = applicationId;
	}
}
