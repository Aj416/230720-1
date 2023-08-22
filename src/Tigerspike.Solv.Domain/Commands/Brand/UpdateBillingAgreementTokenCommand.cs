using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class UpdateBillingAgreementTokenCommand : Command
	{
		public Guid BrandId { get; }

		public string BillingAgreementToken { get; }

		public UpdateBillingAgreementTokenCommand(Guid brandId, string billingAgreementToken)
		{
			BrandId = brandId;
			BillingAgreementToken = billingAgreementToken;
		}

		public override bool IsValid() => BrandId != Guid.Empty && !string.IsNullOrEmpty(BillingAgreementToken);
	}
}