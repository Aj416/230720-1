using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateBrandPaymentAccountCommand : Command
	{
		public Guid BrandId { get; }

		public string BillingAgreementToken { get; }

		public UpdateBrandPaymentAccountCommand(Guid brandId, string billingAgreementToken)
		{
			BrandId = brandId;
			BillingAgreementToken = billingAgreementToken;
		}

		public override bool IsValid() => BrandId != Guid.Empty && !string.IsNullOrEmpty(BillingAgreementToken);
	}
}