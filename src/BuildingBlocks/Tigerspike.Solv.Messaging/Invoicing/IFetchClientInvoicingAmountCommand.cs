using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchClientInvoicingAmountCommand
	{
		/// <summary>
		/// Gets or sets from date.
		/// </summary>
		DateTime From { get; set; }

		/// <summary>
		/// Gets or sets to date.
		/// </summary>
		DateTime To { get; set; }

		/// <summary>
		/// Gets or sets brand identifier.
		/// </summary>
		Guid BrandId { get; set; }
	}
}
