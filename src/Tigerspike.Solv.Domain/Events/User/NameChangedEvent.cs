using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class NameChangedEvent : Event
	{
		public Guid AdvocateId { get; }
		public string FirstName { get; }
		public string LastName { get; }

		public NameChangedEvent(Guid advocateId, string firstName, string lastName)
		{
			AdvocateId = advocateId;
			FirstName = firstName;
			LastName = lastName;
		}
	}
}
