using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateIdentityVerificationStatusUpdatedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocateIdentityVerificationStatusUpdatedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}