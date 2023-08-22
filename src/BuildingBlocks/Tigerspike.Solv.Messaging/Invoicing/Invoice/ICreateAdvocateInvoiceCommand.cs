using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface ICreateAdvocateInvoiceCommand
	{
		Guid AdvocateId { get; set; }

		Guid InvoicingCycleId { get; set; }
	}
}
