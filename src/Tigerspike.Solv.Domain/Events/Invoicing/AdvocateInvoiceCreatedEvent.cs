using System;

namespace Tigerspike.Solv.Domain.Events
{
	/// <summary>
	/// Indicates that an invoice has been created
	/// </summary>
	public class AdvocateInvoiceCreatedEvent
	{
		/// <summary>
		/// The id of the advocate invoice that was created.
		/// </summary>
		public Guid InvoiceId { get; set; }

		/// <summary>
		/// The advocate id that the invoice was generated for.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// The start date of the invoicing cycle that the invoice falls into.
		/// </summary>
		public DateTime From { get; set; }

		/// <summary>
		/// The end date of the invoicing cycle that the invoice falls into.
		/// </summary>
		public DateTime To { get; set; }

		/// <summary>
		/// The number of tickets that were included in the invoice (regardles from the brand).
		/// </summary>
		public int TicketsCount { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected AdvocateInvoiceCreatedEvent()
		{

		}

		public AdvocateInvoiceCreatedEvent(Guid invoiceId, Guid advocateId, DateTime from, DateTime to, int ticketsCount)
		{
			InvoiceId = invoiceId;
			AdvocateId = advocateId;
			From = from;
			To = to;
			TicketsCount = ticketsCount;
		}
	}
}
