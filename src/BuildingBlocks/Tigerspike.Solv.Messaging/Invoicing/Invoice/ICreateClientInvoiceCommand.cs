using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface ICreateClientInvoiceCommand
	{
		Guid BrandId { get; set; }

		Guid InvoicingCycleId { get; set; }
	}
}
