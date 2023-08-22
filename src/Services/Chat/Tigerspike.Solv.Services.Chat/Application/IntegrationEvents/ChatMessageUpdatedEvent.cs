using System;
using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Services.Chat.Application.IntegrationEvents
{
	public class ChatMessageUpdatedEvent : IChatMessageUpdatedEvent
	{
		/// <inheritdoc />
		public Guid Id { get; set; }

		/// <inheritdoc />
		public Guid ConversationId { get; set; }

		/// <inheritdoc />
		public string ThreadId { get; set; }

		/// <inheritdoc />
		public Guid CustomerId { get; set; }

		/// <inheritdoc />
		public Guid? AdvocateId { get; set; }

		/// <inheritdoc />
		public Guid AuthorId { get; set; }

		/// <inheritdoc />
		public string AuthorFirstName { get; set; }

		/// <inheritdoc />
		public string Message { get; set; }

		/// <inheritdoc />
		public int MessageType { get; set; }

		/// <inheritdoc />
		public int SenderType { get; set; }

		/// <inheritdoc />
		public DateTime CreatedDate { get; set; }

		/// <inheritdoc />
		public DateTime Timestamp { get; set; }

		/// <inheritdoc />
		public Guid ClientMessageId { get; set; }
		
		/// <inheritdoc />
		public List<int> RelevantTo { get; set; }
		
		/// <inheritdoc />
		public IChatAction Action { get; set; }
	}
}
