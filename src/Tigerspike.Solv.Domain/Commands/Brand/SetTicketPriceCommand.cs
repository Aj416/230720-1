using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class SetTicketPriceCommand : Command<Unit>
	{
		/// <summary>
		/// The brand id
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// The new price
		/// </summary>
		public decimal Price { get; }

		/// <summary>
		/// Author of the change
		/// </summary>
		public Guid UserId { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketPriceCommand(Guid brandId, decimal price, Guid userId)
		{
			BrandId = brandId;
			Price = price;
			UserId = userId;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty;

	}
}