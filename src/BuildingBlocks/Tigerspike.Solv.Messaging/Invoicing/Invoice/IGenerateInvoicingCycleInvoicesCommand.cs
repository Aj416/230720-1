using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IGenerateInvoicingCycleInvoicesCommand
	{
		Guid InvoicingCycleId { get; set; }

		DateTime From { get; set; }

		DateTime To { get; set; }
	}
}
