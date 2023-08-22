using System;
using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class FetchAdvocateInfoCommand : IFetchAdvocateInfoCommand
	{
		public IEnumerable<Guid> AdvocateIds { get; set; }

		public FetchAdvocateInfoCommand(IEnumerable<Guid> advocateIds) => AdvocateIds = advocateIds;
	}
}
