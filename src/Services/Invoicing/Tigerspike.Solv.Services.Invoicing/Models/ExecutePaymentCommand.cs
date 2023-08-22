using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class ExecutePaymentCommand : IExecutePaymentCommand
	{
		public string RequestId { get; set; }
		public string TrackingId { get; set; }
		public string PaymentAccountId { get; set; }
		public string BillingAgreementId { get; set; }
		public decimal Amount { get; set; }
		public string CurrencyCode { get; set; }
		public string PlatformPaymentAccountId { get; set; }
		public string InvoiceId { get; set; }
		public string Description { get; set; }
		public IList<(string, decimal)> Items { get; set; }

		public ExecutePaymentCommand(string requestId, string trackingId, string paymentAccountId, string billingAgreementId, decimal amount, string currencyCode, string platformPaymentAccountId, string invoiceId, string description, IList<(string, decimal)> items)
		{
			RequestId = requestId;
			TrackingId = trackingId;
			PaymentAccountId = paymentAccountId;
			BillingAgreementId = billingAgreementId;
			Amount = amount;
			CurrencyCode = currencyCode;
			PlatformPaymentAccountId = platformPaymentAccountId;
			InvoiceId = invoiceId;
			Description = description;
			Items = items;
		}
	}
}
