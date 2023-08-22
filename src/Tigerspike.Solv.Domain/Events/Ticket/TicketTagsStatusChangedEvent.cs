using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	/// <summary>
	/// The ticket tagging completion event model
	/// </summary>
	public class TicketTagsStatusChangedEvent : Event
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
		/// The reference id
		/// </summary>
		public string ReferenceId { get; }

		/// <summary>
		/// The advocate id at the time of closure
		/// (could have been changed if a Super solver got in the Middle)
		/// </summary>
		/// <value></value>
		public Guid? AdvocateId { get; }

		/// <summary>
		/// The advocate first name
		/// </summary>
		public string AdvocateFirstName { get; set; }

		/// <summary>
		/// The advocate CSAT score
		/// </summary>
		public decimal? AdvocateCsat { get; set; }

		/// <summary>
		/// The thread id
		/// </summary>
		public string ThreadId { get; }

		/// <summary>
		/// A flag to indicate whether the tagging is complete
		/// </summary>
		public bool IsTaggingCompleted { get; }

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="brandId"></param>
		/// <param name="referenceId"></param>
		/// <param name="threadId"></param>
		/// <param name="advocateId"></param>
		/// <param name="advocateFirstName"></param>
		/// <param name="advocateCsat"></param>
		/// <param name="isTaggingCompleted"></param>
		public TicketTagsStatusChangedEvent(Guid ticketId, Guid brandId, string referenceId, string threadId, 
			Guid? advocateId, string advocateFirstName, decimal? advocateCsat, bool isTaggingCompleted)
		{
			TicketId = ticketId;
			BrandId = brandId;
			ReferenceId = referenceId;
			ThreadId = threadId;
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
			IsTaggingCompleted = isTaggingCompleted;
		}
    }
}