using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	/// <summary>
	/// Flag that the advocate has done the induction for a brand.
	/// </summary>
	public class CompleteInductionCommand : Command<Unit>
	{
		/// <summary>
		/// The advocate who has done the induction
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The brand that advocate has done its induction
		/// </summary>
		public Guid BrandId { get; set; }

		public CompleteInductionCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}