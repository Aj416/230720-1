using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateAdvocateCommand : Command<Unit>
	{
		public Guid AdvocateId { get; private set; }

		public string Email { get; private set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public string Phone { get; private set; }

		public string CountryCode { get; private set; }

		public string Source { get; private set; }

		public bool InternalAgent { get; private set; }

		public bool Verified { get; private set; }

		public bool SuperSolver { get; private set; }

		public CreateAdvocateCommand(Guid advocateId, string firstName, string lastName, string email, string phone, string countryCode, string source, bool internalAgent, bool verified, bool superSolver)
		{
			AdvocateId = advocateId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			CountryCode = countryCode;
			Source = source;
			InternalAgent = internalAgent;
			Verified = verified;
			SuperSolver = superSolver;
		}

		public override bool IsValid()
		{
			return
				AdvocateId != Guid.Empty &&
				!string.IsNullOrEmpty(Email) &&
				!string.IsNullOrEmpty(FirstName);
			// TODO: Removed the check, because we have existing data that might supplied only first name.
			// && !string.IsNullOrEmpty(Phone)
			// && !string.IsNullOrEmpty(LastName);
		}
	}
}