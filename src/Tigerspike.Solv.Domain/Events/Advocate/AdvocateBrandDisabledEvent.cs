using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateBrandDisabledEvent : Event
	{
		public Guid AdvocateId { get; }
		public Guid BrandId { get; }

		public AdvocateBrandDisabledEvent(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}