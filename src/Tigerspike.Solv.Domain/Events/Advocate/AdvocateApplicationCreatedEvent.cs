using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateApplicationCreatedEvent : Event
	{
		public Guid ApplicationId { get; private set; }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string Email { get; private set; }
		public bool InternalAgent { get; private set; }

		public AdvocateApplicationCreatedEvent(Guid applicationId, string firstName, string lastName, string email, bool internalAgent)
		{
			ApplicationId = applicationId;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			InternalAgent = internalAgent;
		}
	}
}