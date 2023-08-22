using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketInfoCommand
	{
		Guid? ClientInvoiceId { get; set; }

		Guid? AdvocateInvoiceId { get; set; }
	}
}
