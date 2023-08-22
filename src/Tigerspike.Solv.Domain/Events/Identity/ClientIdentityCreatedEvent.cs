using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class ClientIdentityCreatedEvent : Event
	{
		public Guid UserId { get; set; }
		public Guid BrandId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }

		public ClientIdentityCreatedEvent(Guid userId, Guid brandId, string firstName, string lastName, string email, string phone)
		{
			UserId = userId;
			BrandId = brandId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
		}
	}
}