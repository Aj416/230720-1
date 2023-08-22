using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IStartInvoicingCycleResult
	{
		bool Success { get; set; }

		Guid Id { get; set; }
	}
}
