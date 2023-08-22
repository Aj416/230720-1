using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IExecutePaymentCommand
	{
		string RequestId { get; set; }

		string TrackingId { get; set; }

		string PaymentAccountId { get; set; }

		string BillingAgreementId { get; set; }

		decimal Amount { get; set; }

		string CurrencyCode { get; set; }

		string PlatformPaymentAccountId { get; set; }

		string InvoiceId { get; set; }

		string Description { get; set; }

		IList<(string, decimal)> Items { get; set; } 
	}
}
