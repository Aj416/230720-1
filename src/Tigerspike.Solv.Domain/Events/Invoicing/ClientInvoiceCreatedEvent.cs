using System;

namespace Tigerspike.Solv.Domain.Events
{
	/// <summary>
	/// Indicates that an invoice has been created
	/// </summary>
	public class ClientInvoiceCreatedEvent
	{
		/// <summary>
		/// The id of the invoice that was created
		/// </summary>
		public Guid InvoiceId { get; set; }

		/// <summary>
		/// The brand id that the invoice was generated for.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The start date of the invoicing cycle that the invoice falls into.
		/// </summary>
		public DateTime From { get; set; }

		/// <summary>
		/// The end date of the invoicing cycle that the invoice falls into.
		/// </summary>
		public DateTime To { get; set; }

		/// <summary>
		/// The number of tickets that were included in the invoice.
		/// </summary>
		public int TicketsCount { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected ClientInvoiceCreatedEvent()
		{

		}

		public ClientInvoiceCreatedEvent(Guid invoiceId, Guid brandId, DateTime from, DateTime to, int ticketsCount)
		{
			InvoiceId = invoiceId;
			BrandId = brandId;
			From = from;
			To = to;
			TicketsCount = ticketsCount;
		}
	}
}
