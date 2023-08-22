using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.User
{
	public class UserUnblockedEvent : Event
	{
		public Guid UserId { get; }

		public UserUnblockedEvent(Guid userId) => UserId = userId;
	}
}
