using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchAdvocatesToInvoiceCommand : IFetchAdvocatesToInvoiceCommand
	{
		/// <inheritdoc />
		public DateTime From { get; set; }

		/// <inheritdoc />
		public DateTime To { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="from">From date range.</param>
		/// <param name="to">To date range.</param>
		public FetchAdvocatesToInvoiceCommand(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}
	}
}
