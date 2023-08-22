using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.User
{
	public class UserBlockedEvent : Event
	{
		public Guid UserId { get; }

		public UserBlockedEvent(Guid userId) => UserId = userId;
	}
}
