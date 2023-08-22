using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateIdentityCreatedEvent : Event
	{
		public Guid UserId { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Phone { get; set; }

		public string CountryCode { get; set; }

		public bool InternalAgent { get; set; }
		public string Source { get; set; }

		public string Role { get; set; }

		public AdvocateIdentityCreatedEvent(Guid userId, string firstName, string lastName, string email, string phone, string countryCode, string source, bool internalAgent, string role)
		{
			UserId = userId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			CountryCode = countryCode;
			InternalAgent = internalAgent;
			Source = source;
			Role = role;
		}
	}
}