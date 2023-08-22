using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Identity
{
	public class UnblockUserCommand : Command<Unit>
	{
		public UnblockUserCommand(Guid userId) => UserId = userId;

		public Guid UserId { get; set; }

		public override bool IsValid() => UserId != Guid.Empty;
	}
}
