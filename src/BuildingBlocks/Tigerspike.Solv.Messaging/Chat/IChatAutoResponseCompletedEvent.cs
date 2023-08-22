using System;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface IChatAutoResponseCompletedEvent
	{
		/// <summary>
		/// The conversation id.
		/// </summary>
		public Guid ConversationId { get; set; }

		/// <summary>
		/// The response type.
		/// </summary>
		public int ResponseType { get; set; }
	}
}