using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchBrandBillingDetailCommand
	{
		/// <summary>
		/// Gets or sets brand id.
		/// </summary>
		Guid BrandId { get; set; }
	}
}
