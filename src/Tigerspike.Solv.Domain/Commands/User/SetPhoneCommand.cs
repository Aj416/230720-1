using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetPhoneCommand : Command<Unit>
	{
		public SetPhoneCommand(Guid userId, string phone)
		{
			UserId = userId;
			Phone = phone;
		}

		public Guid UserId { get; set; }

		public string Phone { get; private set; }

		public override bool IsValid() => UserId != Guid.Empty && !string.IsNullOrEmpty(Phone);
	}
}