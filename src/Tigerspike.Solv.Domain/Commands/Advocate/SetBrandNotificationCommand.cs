using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetBrandNotificationCommand : Command<bool>
	{
		public Guid AdvocateId { get; set; }

		public SetBrandNotificationCommand(Guid userId)
		{
			AdvocateId = userId;
		}

		public override bool IsValid()
		{
			return AdvocateId != Guid.Empty;
		}
	}
}