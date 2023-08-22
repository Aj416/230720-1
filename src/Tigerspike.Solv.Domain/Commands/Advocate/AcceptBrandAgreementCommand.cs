using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Flag that the advocate has accepted brand agreement
	/// </summary>
	public class AcceptBrandAgreementCommand : Command<Unit>
	{
		/// <summary>
		/// The advocate who accepts brand agreement
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The brand that advocate has accepted agreement
		/// </summary>
		public Guid BrandId { get; set; }

		public AcceptBrandAgreementCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}