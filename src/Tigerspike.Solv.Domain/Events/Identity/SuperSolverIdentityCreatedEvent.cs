using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class SuperSolverIdentityCreatedEvent : Event
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CountryCode { get; set; }
		public string Phone { get; set; }

		public SuperSolverIdentityCreatedEvent(Guid userId, string firstName, string lastName, string email, string countryCode, string phone)
		{
			UserId = userId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			CountryCode = countryCode;
			Phone = phone;
		}
	}
}