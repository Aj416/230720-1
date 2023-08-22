using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class TicketStatusHistory
	{
		public TicketStatusHistory(Guid ticketId, TicketStatusEnum status, DateTime createdDate, Guid? advocateId, TicketLevel level)
		{
			TicketId = ticketId;
			Status = status;
			AdvocateId = advocateId;
			CreatedDate = createdDate;
			Level = level;
		}

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private TicketStatusHistory() { }

		public Guid Id { get; private set; }

		public Guid TicketId { get; private set; }

		public TicketStatusEnum Status { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		public Guid? AdvocateId { get; private set; }

		public Advocate Advocate { get; private set; }
		public TicketLevel Level { get; private set; }
	}
}