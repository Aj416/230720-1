using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
		public class PayClientInvoiceCommand : Command<Unit>
	{
		public Guid ClientInvoiceId { get; }

		public PayClientInvoiceCommand(Guid clientInvoiceId)
		{
			ClientInvoiceId = clientInvoiceId;
		}

		public override bool IsValid() => ClientInvoiceId != Guid.Empty;
	}
}