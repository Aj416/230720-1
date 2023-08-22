using System;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	public class CreateAdvocateInvoiceCommand
	{
		public Guid AdvocateId { get; set; }

		public Guid InvoicingCycleId { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected CreateAdvocateInvoiceCommand()
		{

		}
		public CreateAdvocateInvoiceCommand(Guid brandId, Guid invoicingCycleId)
		{
			InvoicingCycleId = invoicingCycleId;
			AdvocateId = brandId;
		}

		public bool IsValid() => AdvocateId != Guid.Empty && InvoicingCycleId != Guid.Empty;
	}
}