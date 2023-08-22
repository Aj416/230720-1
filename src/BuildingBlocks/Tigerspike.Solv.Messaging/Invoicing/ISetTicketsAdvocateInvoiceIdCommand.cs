using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface ISetTicketsAdvocateInvoiceIdCommand
	{
		/// <summary>
		/// AdvocateInvoice identifier.
		/// </summary>
		Guid AdvocateInvoiceId { get; set; }

		/// <summary>
		/// Advocate identifier.
		/// </summary>
		Guid AdvocateId { get; set; }

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
