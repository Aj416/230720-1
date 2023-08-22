using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SendVerificationMailCommand : Command<Unit>
	{
		public SendVerificationMailCommand(Guid userId)
		{
			UserId = userId;
		}

		public Guid UserId { get; set; }

		public override bool IsValid() => UserId != Guid.Empty;
	}
}