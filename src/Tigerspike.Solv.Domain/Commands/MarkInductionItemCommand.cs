using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class MarkInductionItemCommand : Command<Unit>
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public Guid ItemId { get; }

		public MarkInductionItemCommand(Guid advocateId, Guid brandId, Guid itemId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
			ItemId = itemId;
		}

		public override bool IsValid()
		{
			return AdvocateId != Guid.Empty && BrandId != Guid.Empty && ItemId != Guid.Empty;
		}
	}
}