using System;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class ConversationModel
	{
		public string Id { get; set; }

		public string CustomerId { get; set; }

		public string AdvocateId { get; set; }

		public string AdvocateFirstName { get; set; }

		public decimal AdvocateCsat { get; set; }

		public Guid BrandId { get; set; }

		public TicketTransportType TransportType { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime LastMessageTimeStamp { get; set; }

		public DateTime LastReadMessageTimeStamp { get; set; }

		public string ThreadId { get; set; }
	}
}