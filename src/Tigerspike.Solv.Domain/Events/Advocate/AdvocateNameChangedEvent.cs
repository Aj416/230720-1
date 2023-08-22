using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateNameChangedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocateNameChangedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}