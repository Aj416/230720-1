using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IFetchAdvocateInfoCommand
	{
		/// <summary>
		/// List of advocate  ids.
		/// </summary>
		IEnumerable<Guid> AdvocateIds { get; set; }
	}
}
