using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetAdvocateApplicationCompletedEmailSentCommand : Command
	{
		public Guid ApplicationId { get; private set; }

		public SetAdvocateApplicationCompletedEmailSentCommand(Guid applicationId)
		{
			ApplicationId = applicationId;
		}

		public override bool IsValid()
		{
			return ApplicationId != Guid.Empty;
		}
	}
}