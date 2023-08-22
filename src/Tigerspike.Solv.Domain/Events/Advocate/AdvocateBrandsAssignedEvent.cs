using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events
{
	public class AdvocateBrandsAssignedEvent : Event
	{
		public Guid AdvocateId { get; }
		public string AdvocateFirstName { get; }
		public string AdvocateEmail { get; }

		public AdvocateBrandsAssignedEvent(Guid advocateId, string advocateFirstName, string advocateEmail)
		{
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			AdvocateEmail = advocateEmail;
		}
	}
}
