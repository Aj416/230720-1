using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	/// <summary>
	/// The disable ticket tag event model
	/// </summary>
	public class TicketTagsDisabledEvent : Event
	{
		/// <summary>
		/// The ticket id
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The brand id
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// The advocate id 
		/// </summary>
		/// <value></value>
		public Guid AdvocateId { get; }

		/// <summary>
		/// The tags to be disabled
		/// </summary>
		public List<Guid> DisabledTags { get; }

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="brandId"></param>
		/// <param name="advocateId"></param>
		/// <param name="disabledTags"></param>
		public TicketTagsDisabledEvent(Guid ticketId, Guid brandId, Guid advocateId, List<Guid> disabledTags) 
		{
			TicketId = ticketId;
			BrandId = brandId;
			AdvocateId = advocateId;
			DisabledTags = disabledTags;
		}
	}
}