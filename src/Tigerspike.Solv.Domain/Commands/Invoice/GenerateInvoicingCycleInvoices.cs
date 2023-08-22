using System;
using MassTransit;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	/// <inheritdoc/>
	public class GenerateInvoicingCycleInvoices : IConsumer
	{
		public Guid  InvoicingCycleId { get; set; }

		public DateTime From { get; set; }

		public DateTime To { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected GenerateInvoicingCycleInvoices()
		{

		}

		public GenerateInvoicingCycleInvoices(Guid id, DateTime from, DateTime to)
		{
			InvoicingCycleId = id;
			From = from;
			To = to;
		}
	}
}