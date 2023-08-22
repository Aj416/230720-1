using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class ChangeAdvocateNameCommand : Command<Unit>
	{
		public ChangeAdvocateNameCommand(Guid advocateId, string firstName, string lastName)
		{
			AdvocateId = advocateId;
			FirstName = firstName;
			LastName = lastName;
		}

		public Guid AdvocateId { get; set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public override bool IsValid() => AdvocateId != Guid.Empty &&
		!string.IsNullOrEmpty(FirstName) &&
		!string.IsNullOrEmpty(LastName);
	}
}