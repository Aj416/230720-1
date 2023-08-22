using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateInductionCompletedEvent : Event
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public AdvocateInductionCompletedEvent(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}