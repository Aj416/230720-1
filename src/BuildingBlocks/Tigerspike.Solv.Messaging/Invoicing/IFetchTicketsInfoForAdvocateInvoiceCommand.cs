using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchTicketsInfoForAdvocateInvoiceCommand
	{
		/// <summary>
		/// AdvocateInvoiceId Identifier.
		/// </summary>
		Guid AdvocateInvoiceId { get; set; }
	}
}
