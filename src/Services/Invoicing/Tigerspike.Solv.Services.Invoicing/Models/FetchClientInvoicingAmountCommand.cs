using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchClientInvoicingAmountCommand : IFetchClientInvoicingAmountCommand
	{
		/// <inheritdoc />
		public DateTime From { get; set; }

		/// <inheritdoc />
		public DateTime To { get; set; }

		/// <inheritdoc />
		public Guid BrandId { get; set; }

		/// <summary>
		/// Parameterised constructor
		/// </summary>
		/// <param name="from">From date.</param>
		/// <param name="to">To date.</param>
		/// <param name="brandId">Brand identifier.</param>
		public FetchClientInvoicingAmountCommand(DateTime from, DateTime to, Guid brandId)
		{
			From = from;
			To = to;
			BrandId = brandId;
		}
	}
}
