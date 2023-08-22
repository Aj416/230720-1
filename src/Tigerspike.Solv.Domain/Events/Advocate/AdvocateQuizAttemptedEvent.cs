using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateQuizAttemptedEvent : Event
	{
		public Guid AdvocateId { get; }

		public AdvocateQuizAttemptedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}