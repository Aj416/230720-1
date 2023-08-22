using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class SetPricingForNewTicketsCommand : Command<int>
	{
		/// <summary>
		/// The brand id
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// Ticket id list to constraint command scope
		/// </summary>
		public List<Guid> TicketIdList { get; }

		/// <summary>
		/// The new price
		/// </summary>
		public decimal Price { get; }

		/// <summary>
		/// The new fee
		/// </summary>
		public decimal Fee { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetPricingForNewTicketsCommand(Guid brandId, decimal price, decimal fee)
		{
			BrandId = brandId;
			Price = price;
			Fee = fee;
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetPricingForNewTicketsCommand(Guid brandId, decimal price, decimal fee, IEnumerable<Guid> ticketIdList)
			: this(brandId, price, fee)
		{
			TicketIdList = ticketIdList.ToList();
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty;

	}
}