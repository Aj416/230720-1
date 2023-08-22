using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands
{
	public class CreateClientCommand : Command<Unit>
	{
		public Guid UserId { get; private set; }
		public Guid BrandId { get; private set; }

		public string Email { get; private set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public string Phone { get; private set; }

		public CreateClientCommand(Guid userId, Guid brandId, string firstName, string lastName, string email, string phone)
		{
			UserId = userId;
			BrandId = brandId;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
		}

		public override bool IsValid()
		{
			return
				UserId != Guid.Empty &&
				BrandId != Guid.Empty &&
				Email.IsNotEmpty() &&
				FirstName.IsNotEmpty() &&
				LastName.IsNotEmpty();
		}
	}
}