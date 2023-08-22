using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class BrandTicketPriceHistory : ICreatedDate
	{
		public BrandTicketPriceHistory(Guid brandId, decimal ticketPrice, Guid? userId)
		{
			BrandId = brandId;
			TicketPrice = ticketPrice;
			UserId = userId;
		}

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private BrandTicketPriceHistory() { }

		public Guid Id { get; private set; }

		public Guid BrandId { get; private set; }

		public decimal TicketPrice { get; private set; }

		/// <summary>
		/// Author of the change
		/// </summary>
		public Guid? UserId { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

	}
}