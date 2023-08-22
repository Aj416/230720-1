using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchTicketInfoCommand : IFetchTicketInfoCommand
	{
		public Guid? ClientInvoiceId { get; set; }
		public Guid? AdvocateInvoiceId { get; set; }

		public FetchTicketInfoCommand(Guid? clientInvoiceId, Guid? advocateInvoiceId)
		{
			ClientInvoiceId = clientInvoiceId;
			AdvocateInvoiceId = advocateInvoiceId;
		}
	}
}
