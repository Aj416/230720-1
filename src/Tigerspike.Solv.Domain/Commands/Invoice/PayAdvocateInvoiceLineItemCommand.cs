using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	public class PayAdvocateInvoiceLineItemCommand : Command<Unit>
	{
		public Guid AdvocateInvoiceLineItemId { get; set; }

		public Guid BrandId { get; set; }

		public Guid AdvocateId { get; set; }

		public Guid AdvocateInvoiceId { get; set; }

		public string BrandPaymentAccountId { get; set; }

		public string SolverPaymentAccountId { get; set; }

		public string BillingAgreementId { get; set; }

		public decimal Amount { get; set; }

		public string CurrencyCode { get; set; }

		public string Description { get; set; }

		public IList<(string, decimal)> Breakdown { get; set; }

		public PayAdvocateInvoiceLineItemCommand()
		{
		}

		public override bool IsValid() =>
			AdvocateInvoiceLineItemId != Guid.Empty &&
			BrandId != Guid.Empty &&
			AdvocateId != Guid.Empty &&
			AdvocateInvoiceId != Guid.Empty &&
			BrandPaymentAccountId.IsNotEmpty() &&
			SolverPaymentAccountId.IsNotEmpty() &&
			BillingAgreementId.IsNotEmpty() &&
			Amount > 0m &&
			CurrencyCode.IsNotEmpty() &&
			Description.IsNotEmpty() &&
			Breakdown.Count > 0;
	}
}