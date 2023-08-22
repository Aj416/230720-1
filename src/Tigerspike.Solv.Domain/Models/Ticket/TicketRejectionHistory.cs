using System;
using System.Collections.Generic;
using System.Linq;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represent a relation between a ticket and a rejection reason.
	/// </summary>
	public class TicketRejectionHistory
	{
		/// <summary>
		/// The id of the ticket rejection history.
		/// It is the same id as the TicketStatusHistory that has the Reserved status.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The list of reasons that this rejection history was created for.
		/// It returns the joining entity TicketRejectionReason that refers to the actual RejectionReason
		/// </summary>
		public List<TicketRejectionReason> Reasons { get; private set; }

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private TicketRejectionHistory() { }

		public TicketRejectionHistory(Guid ticketId, Guid reservedStatusId, int[] rejectReasonIds, DateTime createdDate)
		{
			Id = reservedStatusId;
			TicketId = ticketId;
			Reasons = rejectReasonIds.Select(reasonId => new TicketRejectionReason(Id, reasonId)).ToList();
			CreatedDate = createdDate;
		}
	}

	/// <summary>
	/// A joining entity between ticket and rejection reasons.
	/// Has no business value and thus it should be only used by EF.
	/// Couldn't make it private though.
	/// </summary>
	public class TicketRejectionReason
	{
		public Guid TicketRejectionHistoryId { get; private set; }

		public int RejectionReasonId { get; private set; }

		public RejectionReason RejectionReason { get; private set; }

		public TicketRejectionReason() { }

		public TicketRejectionReason(Guid ticketRejectionHistoryId, int rejectionReasonId)
		{
			TicketRejectionHistoryId = ticketRejectionHistoryId;
			RejectionReasonId = rejectionReasonId;
		}
	}
}
