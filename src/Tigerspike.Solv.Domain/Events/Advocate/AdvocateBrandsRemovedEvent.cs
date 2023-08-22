using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateBrandsRemovedEvent : Event
	{
		public Guid AdvocateId { get; }
		public string AdvocateEmail { get; set; }
		public string AdvocateFirstName { get; set; }
		public IEnumerable<Guid> BrandIds { get; }

		public AdvocateBrandsRemovedEvent(Guid advocateId, string advocateEmail, string advocateFirstName, IEnumerable<Guid> brandIds)
		{
			AdvocateId = advocateId;
			BrandIds = brandIds;
			AdvocateEmail = advocateEmail;
			AdvocateFirstName = advocateFirstName;
		}
	}
}
