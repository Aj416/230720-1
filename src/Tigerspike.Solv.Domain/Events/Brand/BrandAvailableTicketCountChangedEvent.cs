using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events
{
	public class BrandAvailableTicketCountChangedEvent : Event
	{
		public Guid BrandId { get; }
		public TicketLevel Level { get; }

		public BrandAvailableTicketCountChangedEvent(Guid brandId, TicketLevel level)
		{
			BrandId = brandId;
			Level = level;
		}
	}
}