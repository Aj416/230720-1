using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetGuideLineCommand : Command<Unit>
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public SetGuideLineCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid()
		{
			return AdvocateId != Guid.Empty && BrandId != Guid.Empty;
		}
	}
}