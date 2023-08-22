using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketsInfoForAdvocateInvoiceResult : IResult
	{
		/// <summary>
		/// Result set.
		/// </summary>
		IEnumerable<(Guid id, Guid brandId, decimal price)> InvoicedTickets { get; set; }
	}
}
