using System;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Chat.Domain
{
	public class  Conversation
	{
		[HashKey]
		public string Id { get; set; }

		public string CustomerId { get; set; }

		public string AdvocateId { get; set; }

		public string AdvocateFirstName { get; set; }

		public decimal AdvocateCsat { get; set; }

		public Guid BrandId { get; set; }

		public TicketTransportType TransportType { get; set; }

		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The time stamp of the last message that has been added to the conversation
		/// </summary>
		public DateTime LastMessageTimeStamp { get; set; }

		/// <summary>
		/// The time stamp of the last message that the advocate has read.
		/// </summary>
		public DateTime LastReadMessageTimeStamp { get; set; }

		/// <summary>
		/// The thread id from the messenger.
		/// </summary>
		public string ThreadId { get; set; }
	}
}