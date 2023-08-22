using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateContractAgreedEvent : Event
	{
		public Guid AdvocateId { get; }
		public Guid BrandId { get; }

		public AdvocateContractAgreedEvent(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}
	}
}