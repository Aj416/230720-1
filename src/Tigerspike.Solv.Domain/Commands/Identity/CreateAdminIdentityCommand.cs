using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateAdminIdentityCommand : Command<Guid?>
	{
		public Guid UserId { get; private set; }
		public string Email { get; private set; }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string Password { get; private set; }

		public CreateAdminIdentityCommand(Guid userId, string firstName, string lastName, string email, string password)
		{
			UserId = userId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Password = password;
		}
		public override bool IsValid()
		{
			return
				UserId != Guid.Empty &&
				Email.IsNotEmpty() &&
				FirstName.IsNotEmpty() &&
				LastName.IsNotEmpty() &&
				Password.IsNotEmpty();
		}

	}
}