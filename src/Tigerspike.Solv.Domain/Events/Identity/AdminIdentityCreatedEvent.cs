using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdminIdentityCreatedEvent : Event
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public AdminIdentityCreatedEvent(Guid userId, string firstName, string lastName, string email)
		{
			UserId = userId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
		}
	}
}