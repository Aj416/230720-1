using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class ChangeNameCommand : Command<Unit>
	{
		public ChangeNameCommand(Guid userId, string firstName, string lastName)
		{
			UserId = userId;
			FirstName = firstName;
			LastName = lastName;
		}

		public Guid UserId { get; set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public override bool IsValid() => UserId != Guid.Empty &&
		!string.IsNullOrEmpty(FirstName) &&
		!string.IsNullOrEmpty(LastName);
	}
}