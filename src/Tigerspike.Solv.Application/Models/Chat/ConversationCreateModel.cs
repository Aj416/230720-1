using System;

namespace Tigerspike.Solv.Application.Models.Chat
{
	public class ConversationCreateModel
	{
		public Guid TicketId { get; set; }

		public Guid CustomerId { get; set; }

		public Guid BrandId { get; set; }

		public int TransportType { get; set; }

		public string ThreadId { get; set; }
	}
}