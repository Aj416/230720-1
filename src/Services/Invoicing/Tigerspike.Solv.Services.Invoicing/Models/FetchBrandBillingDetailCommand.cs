using System;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchBrandBillingDetailCommand : IFetchBrandBillingDetailCommand
	{
		/// <inheritdoc />
		public Guid BrandId { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="brandId">Brand identifier.</param>
		public FetchBrandBillingDetailCommand(Guid brandId) => BrandId = brandId;
	}
}
