using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateAdminCommand : Command<Unit>
	{
		public Guid UserId { get; private set; }
		public string Email { get; private set; }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }

		public CreateAdminCommand(Guid userId, string firstName, string lastName, string email)
		{
			UserId = userId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
		}

		public override bool IsValid()
		{
			return
				UserId != Guid.Empty &&
				Email.IsNotEmpty() &&
				FirstName.IsNotEmpty() &&
				LastName.IsNotEmpty();
		}
	}
}