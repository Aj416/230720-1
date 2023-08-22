using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetVideoWatchedCommand : Command
	{
		public Guid AdvocateId { get; set; }

		public SetVideoWatchedCommand(Guid advocateId)
		{
			AdvocateId = advocateId;
		}
		
		public override bool IsValid()
		{
			return AdvocateId != Guid.Empty;
		}
	}
}