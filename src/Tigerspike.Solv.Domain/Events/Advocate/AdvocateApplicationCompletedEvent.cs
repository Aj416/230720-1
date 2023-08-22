using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationCompletedEvent : Event
	{
		public Guid ApplicationId { get; }
		
		public string Email { get; }

		public AdvocateApplicationCompletedEvent(Guid applicationId, string email)
		{
			ApplicationId = applicationId;
			Email = email;
		}
	}
}