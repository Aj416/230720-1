using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateAdvocateCsatCommand : Command<Unit>
	{
		public Guid AdvocateId { get; }

		public Guid BrandId { get; }

		public UpdateAdvocateCsatCommand(Guid advocateId, Guid brandId)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && BrandId != Guid.Empty;
	}
}