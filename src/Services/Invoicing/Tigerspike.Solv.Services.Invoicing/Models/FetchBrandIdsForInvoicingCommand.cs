using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchBrandIdsForInvoicingCommand : IFetchBrandIdsForInvoicingCommand
	{
		/// <inheritdoc />
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="advocateId">Advocate identifier.</param>
		public FetchBrandIdsForInvoicingCommand(Guid advocateId) => AdvocateId = advocateId;
	}
}
