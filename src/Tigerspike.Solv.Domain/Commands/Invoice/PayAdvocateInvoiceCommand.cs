using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
		public class PayAdvocateInvoiceCommand : Command<Unit>
	{
		public Guid AdvocateInvoiceId { get; }

		public PayAdvocateInvoiceCommand(Guid advocteInvoiceId)
		{
			AdvocateInvoiceId = advocteInvoiceId;
		}

		public override bool IsValid() => AdvocateInvoiceId != Guid.Empty;
	}
}