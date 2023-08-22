using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface IChatMessageUpdatedEvent
	{
		/// <summary>
		/// The message id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The conversation id.
		/// </summary>
		public Guid ConversationId { get; set; }

		/// <summary>
		/// The messenger thread of the conversation.
		/// </summary>
		public string ThreadId { get; set; }

		/// <summary>
		/// The customer id.
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The advocate id.
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The author id.
		/// </summary>
		public Guid AuthorId { get; set; }

		/// <summary>
		/// The author first name.
		/// </summary>
		public string AuthorFirstName { get; set; }

		/// <summary>
		/// The content of the message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The message type.
		/// </summary>
		public int MessageType { get; set; }

		/// <summary>
		/// The sender type of the message.
		/// </summary>
		public int SenderType { get; set; }

		/// <summary>
		/// Gets or sets the message created date.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the event timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the client message id.
		/// </summary>
		public Guid ClientMessageId { get; set; }

		/// <summary>
		/// Message relevant to.
		/// </summary>
		public List<int> RelevantTo { get; set; }

		/// <summary>
		/// The chat action.
		/// </summary>
		public IChatAction Action { get; set; }
	}
}