using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetIdentityApplicantCommand : Command
	{
		public Guid AdvocateId { get; set; }
		public string IdentityApplicantId { get; set; }

		public SetIdentityApplicantCommand(Guid advocateId, string identityApplicantId)
		{
			AdvocateId = advocateId;
			IdentityApplicantId = identityApplicantId;
		}

		public override bool IsValid() =>
			AdvocateId != Guid.Empty &&
			IdentityApplicantId.IsNotEmpty();
	}
}