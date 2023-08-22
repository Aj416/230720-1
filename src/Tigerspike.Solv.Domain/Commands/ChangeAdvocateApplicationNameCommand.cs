using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class ChangeAdvocateApplicationNameCommand : Command<Unit>
	{
		public ChangeAdvocateApplicationNameCommand(Guid advocateApplicationId, string firstName, string lastName)
		{
			AdvocateApplicationId = advocateApplicationId;
			FirstName = firstName;
			LastName = lastName;
		}

		public Guid AdvocateApplicationId { get; set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public override bool IsValid() => AdvocateApplicationId != Guid.Empty &&
		!string.IsNullOrEmpty(FirstName) &&
		!string.IsNullOrEmpty(LastName);
	}
}