using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IInvoicingBrandIdResult
	{
		bool IsSuccess { get; set; }

		IEnumerable<Guid> BrandIds { get; set; }
	}
}
