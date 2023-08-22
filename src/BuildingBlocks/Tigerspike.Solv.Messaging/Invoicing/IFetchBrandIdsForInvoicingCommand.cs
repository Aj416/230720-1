using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchBrandIdsForInvoicingCommand
	{
		/// <summary>
		/// Advocate identifier.
		/// </summary>
		Guid AdvocateId { get; set; }
	}
}
