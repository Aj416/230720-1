using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.User
{
	public class MfaResetEvent : Event
	{
		public Guid UserId { get; }
		public string UserEmail { get; }

		public MfaResetEvent(Guid userId, string userEmail)
		{
			UserId = userId;
			UserEmail = userEmail;
		}
	}
}
