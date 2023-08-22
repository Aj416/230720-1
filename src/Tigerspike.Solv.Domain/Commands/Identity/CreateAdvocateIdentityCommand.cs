using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateAdvocateIdentityCommand : Command<Unit>
	{
		public Guid AdvocateId { get; private set; }

		public string Email { get; private set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public string Phone { get; private set; }

		public string CountryCode { get; private set; }

		public string Source { get; private set; }

		public string Password { get; private set; }

		public bool InternalAgent { get; private set; }

		public CreateAdvocateIdentityCommand(Guid advocateId, string firstName, string lastName, string email, string phone, string countryCode, string source, bool internalAgent, string password)
		{
			AdvocateId = advocateId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			CountryCode = countryCode;
			Source = source;
			InternalAgent = internalAgent;
			Password = password;
		}

		public override bool IsValid()
		{
			return AdvocateId != Guid.Empty && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Password);
		}
	}
}