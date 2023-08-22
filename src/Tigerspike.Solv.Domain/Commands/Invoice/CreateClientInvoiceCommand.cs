using System;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	public class CreateClientInvoiceCommand
	{
		public Guid BrandId { get; set; }

		public Guid InvoicingCycleId { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected CreateClientInvoiceCommand()
		{

		}
		public CreateClientInvoiceCommand(Guid brandId, Guid invoicingCycleId)
		{
			InvoicingCycleId = invoicingCycleId;
			BrandId = brandId;
		}

		public bool IsValid() => BrandId != Guid.Empty && InvoicingCycleId != Guid.Empty;
	}
}