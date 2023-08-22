using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocatePaymentAccountUpdatedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocatePaymentAccountUpdatedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}