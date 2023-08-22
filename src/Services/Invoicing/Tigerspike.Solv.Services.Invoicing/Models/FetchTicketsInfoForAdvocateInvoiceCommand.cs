using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchTicketsInfoForAdvocateInvoiceCommand : IFetchTicketsInfoForAdvocateInvoiceCommand
	{
		/// <inheritdoc />
		public Guid AdvocateInvoiceId { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateInvoiceId">Advocate Invoice Identifier.</param>
		public FetchTicketsInfoForAdvocateInvoiceCommand(Guid advocateInvoiceId) => AdvocateInvoiceId = advocateInvoiceId;
	}
}
