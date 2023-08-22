using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateIdsForInvoicingResult : IResult
	{
		/// <summary>
		/// List of advocate ids.
		/// </summary>
		IEnumerable<Guid> AdvocateIds { get; set; }
	}
}
