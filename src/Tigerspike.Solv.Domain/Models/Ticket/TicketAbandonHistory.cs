using System;
using System.Collections.Generic;
using System.Linq;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represent a relation between a ticket and a abandon reason.
	/// </summary>
	public class TicketAbandonHistory
	{
		/// <summary>
		/// The id of the ticket abandon history
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; private set; }
		public Guid? AdvocateId { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The list of reasons that this abandon history was created for.
		/// It returns the joining entity TicketAbandonReason that refers to the actual AbandonReason
		/// </summary>
		public List<TicketAbandonReason> Reasons { get; private set; }

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private TicketAbandonHistory() { }

		public TicketAbandonHistory(Guid ticketId, Guid? advocateId, Guid reservedStatusId, Guid[] abandonReasonIds, DateTime createdDate)
		{
			Id = reservedStatusId;
			AdvocateId = advocateId;
			TicketId = ticketId;
			Reasons = abandonReasonIds.Select(reasonId => new TicketAbandonReason(Id, reasonId)).ToList();
			CreatedDate = createdDate;
		}
	}

	/// <summary>
	/// A joining entity between ticket and abandon reasons.
	/// Has no business value and thus it should be only used by EF.
	/// Couldn't make it private though.
	/// </summary>
	public class TicketAbandonReason
	{
		public Guid TicketAbandonHistoryId { get; private set; }

		public Guid AbandonReasonId { get; private set; }

		public AbandonReason AbandonReason { get; private set; }

		public TicketAbandonReason() { }

		public TicketAbandonReason(Guid ticketAbandonHistoryId, Guid abandonReasonId)
		{
			TicketAbandonHistoryId = ticketAbandonHistoryId;
			AbandonReasonId = abandonReasonId;
		}
	}
}