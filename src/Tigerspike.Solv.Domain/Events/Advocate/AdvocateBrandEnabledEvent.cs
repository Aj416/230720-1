using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateBrandEnabledEvent : Event
	{
		public Guid AdvocateId { get; }
		public Guid BrandId { get; }

		public AdvocateBrandEnabledEvent(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}