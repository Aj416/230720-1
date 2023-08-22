using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The ticket tags disabled model for hub communication
	/// </summary>
    public class TicketTagsDisabledModel
    {
		/// <summary>
		/// The ticket id
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The tags that are to be disabled
		/// </summary>
		public List<Guid> TagIds { get; }

		/// <summary>
		/// Constructor for initialization
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="tagIds"></param>
		public TicketTagsDisabledModel(Guid ticketId, List<Guid> tagIds)
        {
            TicketId = ticketId;
            TagIds = tagIds;
        }
    }
}