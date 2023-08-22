using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchTicketsForAdvocateInvoiceCommand : IFetchTicketsForAdvocateInvoiceCommand
	{
		/// <inheritdoc />
		public DateTime From { get; set; }

		/// <inheritdoc />
		public DateTime To { get; set; }

		/// <inheritdoc />
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="from">From date range.</param>
		/// <param name="to">To date range.</param>
		/// <param name="advocateId">Advocate identifer.</param>
		public FetchTicketsForAdvocateInvoiceCommand(DateTime from, DateTime to, Guid advocateId)
		{
			From = from;
			To = to;
			AdvocateId = advocateId;
		}
	}
}
