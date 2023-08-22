using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocatesToInvoiceCommand
	{
		/// <summary>
		/// From date range.
		/// </summary>
		DateTime From { get; set; }

		/// <summary>
		/// To date range.
		/// </summary>
		DateTime To { get; set; }
	}
}
