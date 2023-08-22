using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Identity
{
	public class ResetMfaCommand : Command<Unit>
	{
		public Guid UserId { get; set; }
		public ResetMfaCommand(Guid userId) => UserId = userId;
		public override bool IsValid() => UserId != Guid.Empty;
	}
}
