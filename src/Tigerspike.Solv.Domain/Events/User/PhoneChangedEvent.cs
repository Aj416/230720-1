using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class PhoneChangedEvent : Event
	{
		public Guid UserId { get; }
		public string Phone { get; }

		public PhoneChangedEvent(Guid userId, string phone)
		{
			UserId = userId;
			Phone = phone;
		}
	}
}
