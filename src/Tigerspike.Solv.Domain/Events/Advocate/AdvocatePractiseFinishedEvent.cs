using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocatePractiseFinishedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocatePractiseFinishedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}