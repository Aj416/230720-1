using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface ISetTicketsClientInvoiceIdCommand
	{
		/// <summary>
		/// ClientInvoice identifier.
		/// </summary>
		Guid ClientInvoiceId { get; set; }

		/// <summary>
		/// Brand identifier.
		/// </summary>
		Guid BrandId { get; set; }

		/// <summary>
		/// From
		/// </summary>
		DateTime From { get; set; }

		/// <summary>
		/// To
		/// </summary>
		DateTime To { get; set; }
	}
}
