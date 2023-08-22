using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketTagsChangedEvent : Event
	{
		public Guid TicketId { get; private set; }
		public Guid BrandId { get; private set; }
		public string ReferenceId { get; private set; }
		public IEnumerable<string> PreviousTags { get; private set; }
		public IEnumerable<string> CurrentTags { get; private set; }
		public int Level { get; private set; }

		public TicketTagsChangedEvent(Guid ticketId, Guid brandId, string referenceId, int level,
			IEnumerable<string> previousTags, IEnumerable<string> currentTags)
		{
			TicketId = ticketId;
			BrandId = brandId;
			ReferenceId = referenceId;
			Level = level;
			PreviousTags = previousTags;
			CurrentTags = currentTags;
		}
	}
}