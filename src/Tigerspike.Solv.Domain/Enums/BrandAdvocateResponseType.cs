using System;

namespace Tigerspike.Solv.Domain.Enums
{
	public enum BrandAdvocateResponseType
	{
		/// <summary>
		/// Auto-response when ticket is accepted by a Level 1 Solver
		/// </summary>
		TicketAcceptedLevel1 = 0,

		/// <summary>
		/// Auto-response when ticket is accepted by a Level 2 Solver, when escalation was caused via tag
		/// </summary>
		TicketAcceptedLevel2TagEscalated = 1,

		/// <summary>
		/// Auto-response when ticket is accepted by a Level 2 Solver, when escalation was not caused via tag
		/// </summary>
		TicketAcceptedLevel2NonTagEscalated = 2,

		/// <summary>
		/// Auto-response when ticket is created
		/// </summary>
		TicketCreated = 3,

		/// <summary>
		/// Returning Customer message for Open tickets
		/// </summary>
		ReturningCustomerTicketOpen = 4,

		/// <summary>
		/// Returning Customer message for In progress tickets
		/// </summary>
		ReturningCustomerTicketInProgress = 5,

		/// <summary>
		/// Returning Customer message for when ticket is auto-abandoned
		/// </summary>
		ReturningCustomerTicketAbandoned = 6,

		/// <summary>
		/// Returning Customer message for when ticket is marked as solved
		/// </summary>
		ReturningCustomerTicketSolved = 7,

		/// <summary>
		/// Returning Customer message for when ticket is reopened
		/// </summary>
		ReturningCustomerTicketReopened = 8,

		/// <summary>
		/// System cut off message and action
		/// </summary>
		TicketCutOff = 9,

		/// <summary>
		/// Chat resumed from notification message for In progress tickets
		/// </summary>
		NotificationResumptionTicketInProgress = 10,

		/// <summary>
		/// Chat resumed from notification message for when ticket is auto-abandoned
		/// </summary>
		NotificationResumptionTicketAbandoned = 11,

		/// <summary>
		/// Chat resumed from notification message for when ticket is marked as solved
		/// </summary>
		NotificationResumptionTicketSolved = 12,

		/// <summary>
		/// Chat resumed from notification message for when ticket is reopened
		/// </summary>
		NotificationResumptionTicketReopened = 13,

		/// <summary>
		/// Chat ticket is concluded (closed, csat/nps done)
		/// </summary>
		[Obsolete("This event is not in use anymore. It's replaced by TicketRating/TicketNpsRated/TicketCsatRated")]
		ChatTicketConcluded = 14,

		/// <summary>
		/// Auto-response when ticket is abandoned
		/// </summary>
		TicketAbandoned = 15,

		/// <summary>
		/// Auto-response when ticket is abandoned and should be escalated
		/// </summary>
		TicketAbandonedEscalation = 16,

		/// <summary>
		/// Auto-response when ticket is escalated
		/// </summary>
		TicketEscalated = 17,

		/// <summary>
		/// When ticket is too long in the open state
		/// </summary>
		TicketOpenTimeoutEscalation = 18,

		/// <summary>
		/// Ticket Rating
		/// </summary>
		TicketRating = 19,

		/// <summary>
		/// Ticket NPS Rated
		/// </summary>
		TicketNpsRated = 20,

		/// <summary>
		/// Ticket Csat Rated
		/// </summary>
		TicketCsatRated = 21,

		/// <summary>
		/// Ticket FeedBacked
		/// </summary>
		TicketFeedBacked = 22,

		/// <summary>
		/// Ticket created system response
		/// </summary>
		TicketCreatedSystemResponse = 23,
	}
}
