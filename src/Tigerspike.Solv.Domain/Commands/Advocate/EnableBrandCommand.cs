using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Flag that the advocate enables a brand.
	/// </summary>
	public class EnableBrandCommand : Command<Unit>
	{
		/// <summary>
		/// Advocate
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Brand
		/// </summary>
		public Guid BrandId { get; set; }

		public EnableBrandCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}