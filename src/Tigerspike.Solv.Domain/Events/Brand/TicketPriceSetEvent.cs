using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class TicketPriceSetEvent : Event
	{
		public Guid BrandId { get; }

		public decimal Price { get; }

		public decimal FeePercentage { get; }

		public TicketPriceSetEvent(Guid brandId, decimal price, decimal feePercentage)
		{
			BrandId = brandId;
			Price = price;
			FeePercentage = feePercentage;
		}
	}
}