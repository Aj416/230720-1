using System;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The ticket tagging status change model for hub communication
	/// </summary>
    public class TicketTaggingStatusChangedModel
    {
		/// <summary>
		/// The conversation id
		/// </summary>
		public Guid ConversationId { get; set; }

		/// <summary>
		/// The advocate details
		/// </summary>
		public AdvocateModel Advocate { get; set; }

		/// <summary>
		/// A flag to indicate whether the tagging is complete
		/// </summary>
		public bool IsTaggingComplete { get; set; }

		/// <summary>
		/// Constructor for initialization
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="isTaggingComplete"></param>
		/// <param name="advocateId"></param>
		/// <param name="advocateFirstName"></param>
		/// <param name="advocateCsat"></param>
		public TicketTaggingStatusChangedModel(Guid conversationId, bool isTaggingComplete, Guid? advocateId, string advocateFirstName = null, decimal? advocateCsat = null)
		{
			ConversationId = conversationId;
			IsTaggingComplete = isTaggingComplete;
			Advocate = advocateId != null ? new AdvocateModel() { Id = advocateId.Value, FirstName = advocateFirstName, Csat = advocateCsat.Value} : null;
		}
    }
}