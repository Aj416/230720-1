using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class PayClientInvoiceCommand : Command<Unit>
	{
		/// <summary>
		/// Gets or sets ClientInvoiceId.
		/// </summary>
		public Guid ClientInvoiceId { get; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="clientInvoiceId">client Invoice Id.</param>
		public PayClientInvoiceCommand(Guid clientInvoiceId) => ClientInvoiceId = clientInvoiceId;

		public override bool IsValid() => ClientInvoiceId != Guid.Empty;
	}
}
