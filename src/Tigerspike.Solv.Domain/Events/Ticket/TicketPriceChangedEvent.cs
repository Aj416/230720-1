using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketPriceChangedEvent : Event
	{
		public Guid TicketId { get; }

		public decimal NewPrice { get; }

		public decimal NewFee { get; }

		public bool IsPractice { get; }

		public TicketPriceChangedEvent(Guid ticketId, decimal newPrice, decimal newFee, bool isPractice)
		{
			TicketId = ticketId;
			NewPrice = newPrice;
			NewFee = newFee;
			IsPractice = isPractice;
		}
	}
}