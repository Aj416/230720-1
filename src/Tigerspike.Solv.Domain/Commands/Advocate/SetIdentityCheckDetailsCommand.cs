using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetIdentityCheckDetailsCommand : Command
	{
		public Guid AdvocateId { get; set; }
		public string IdentityCheckId { get; set; }
		public string IdentityCheckResultUrl { get; set; }

		public SetIdentityCheckDetailsCommand(Guid advocateId, string identityCheckId, string identityCheckResultUrl)
		{
			AdvocateId = advocateId;
			IdentityCheckId = identityCheckId;
			IdentityCheckResultUrl = identityCheckResultUrl;
		}

		public override bool IsValid() =>
			AdvocateId != Guid.Empty &&
			IdentityCheckId.IsNotEmpty();
	}
}