using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchBrandIdsForInvoicingResult : IResult
	{
		/// <summary>
		/// List of brand ids.
		/// </summary>
		IEnumerable<(Guid id, string name)> BrandDetails { get; set; }
	}
}
