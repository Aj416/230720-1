using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class StartAdvocatePracticeCommand : Command
	{
		public Guid AdvocateId { get; private set; }

		public StartAdvocatePracticeCommand(Guid userId) => AdvocateId = userId;

		public override bool IsValid() => AdvocateId != Guid.Empty;
	}
}