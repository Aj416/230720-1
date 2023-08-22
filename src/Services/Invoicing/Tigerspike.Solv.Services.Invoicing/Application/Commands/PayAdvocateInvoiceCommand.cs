using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class PayAdvocateInvoiceCommand : Command<Unit>
	{
		/// <summary>
		/// Gets or sets advocateInvoiceId.
		/// </summary>
		public Guid AdvocateInvoiceId { get; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocteInvoiceId">Advocte InvoiceId.</param>
		public PayAdvocateInvoiceCommand(Guid advocteInvoiceId) => AdvocateInvoiceId = advocteInvoiceId;

		public override bool IsValid() => AdvocateInvoiceId != Guid.Empty;
	}
}
