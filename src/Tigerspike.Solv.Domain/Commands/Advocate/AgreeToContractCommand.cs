using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Flag that the advocate has agreed to the contract of a brand.
	/// </summary>
	public class AgreeToContractCommand : Command<Unit>
	{
		/// <summary>
		/// The advocate who agrees to contract
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The brand that advocate has agreed to it's contract
		/// </summary>
		public Guid BrandId { get; set; }

		public AgreeToContractCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}