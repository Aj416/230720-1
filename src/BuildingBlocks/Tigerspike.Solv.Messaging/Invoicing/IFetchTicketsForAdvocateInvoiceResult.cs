using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketsForAdvocateInvoiceResult : IResult
	{
		/// <summary>
		/// Result set.
		/// </summary>
		IEnumerable<(Guid brandId, string brandName, decimal priceTotal, int ticketsCount)> TicketsByBrand { get; set; }
	}
}
