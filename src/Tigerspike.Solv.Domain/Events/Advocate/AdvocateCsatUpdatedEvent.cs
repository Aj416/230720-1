using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateCsatUpdatedEvent : Event
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public AdvocateCsatUpdatedEvent(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}