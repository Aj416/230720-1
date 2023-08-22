using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetIdentityVerificationStatusCommand : Command
	{
		public Guid AdvocateId { get; set; }
		public IdentityVerificationStatus IdentityVerificationStatus { get; set; }

		public SetIdentityVerificationStatusCommand(Guid advocateId, IdentityVerificationStatus identityVerificationStatus)
		{
			AdvocateId = advocateId;
			IdentityVerificationStatus = identityVerificationStatus;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}