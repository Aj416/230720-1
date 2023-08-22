using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class NewAdvocateBrandsAssignedEvent : Event
	{
		public Guid AdvocateId { get; }

		public NewAdvocateBrandsAssignedEvent(Guid advocateId) => AdvocateId = advocateId;
	}
}