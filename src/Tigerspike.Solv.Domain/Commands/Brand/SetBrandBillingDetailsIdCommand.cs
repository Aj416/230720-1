using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class SetBrandBillingDetailsIdCommand : Command<Unit>
	{
		public Guid BrandId { get; set; }

		public Guid BillingDetailsId { get; set; }

		public SetBrandBillingDetailsIdCommand(Guid brandId, Guid billingDetailsId)
		{
			BrandId = brandId;
			BillingDetailsId = billingDetailsId;
		}

		public override bool IsValid() => BrandId != Guid.Empty && BillingDetailsId != Guid.Empty;
	}
}
