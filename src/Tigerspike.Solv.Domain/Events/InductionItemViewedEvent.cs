using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class InductionItemViewedEvent : Event
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public Guid ItemId { get; }

		public InductionItemViewedEvent(Guid advocateId, Guid brandId, Guid itemId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
			ItemId = itemId;
		}
	}
}