using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketsForAdvocateInvoiceCommand
	{
		/// <summary>
		/// From date range.
		/// </summary>
		DateTime From { get; set; }

		/// <summary>
		/// To date range.
		/// </summary>
		DateTime To { get; set; }

		/// <summary>
		/// Advocate identifier.
		/// </summary>
		Guid AdvocateId { get; set; }
	}
}
