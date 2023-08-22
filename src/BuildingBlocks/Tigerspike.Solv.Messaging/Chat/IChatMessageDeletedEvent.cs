using System;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface IChatMessageDeletedEvent
	{
		/// <summary>
		/// The conversation id.
		/// </summary>
		public Guid ConversationId { get; set; }

		/// <summary>
		/// The message id.
		/// </summary>
		public Guid MessageId { get; set; }

		/// <summary>
		/// The customer id.
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The advocate id.
		/// </summary>
		public Guid? AdvocateId { get; set; }
	}
}