using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketChatResumedEvent : Event
	{
		public Guid TicketId { get; }
		public Guid BrandId { get; }
		public TicketStatusEnum Status { get; }
		public string AdvocateFirstName { get; }
		public string CustomerFirstName { get; }

		public TicketChatResumedEvent(Guid ticketId, Guid brandId, TicketStatusEnum status, string advocateFirstName, string customerFirstName)
		{
			TicketId = ticketId;
			BrandId = brandId;
			Status = status;
			AdvocateFirstName = advocateFirstName;
			CustomerFirstName = customerFirstName;
		}
	}
}