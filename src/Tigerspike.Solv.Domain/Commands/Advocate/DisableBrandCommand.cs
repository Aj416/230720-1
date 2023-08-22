using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Flag that the advocate disables a brand.
	/// </summary>
	public class DisableBrandCommand : Command<Unit>
	{
		/// <summary>
		/// Advocate
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Brand
		/// </summary>
		public Guid BrandId { get; set; }

		public DisableBrandCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}