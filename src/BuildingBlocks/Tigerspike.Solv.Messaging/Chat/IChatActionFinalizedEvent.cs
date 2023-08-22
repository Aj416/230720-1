using System;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface IChatActionFinalizedEvent
	{
		/// <summary>
		/// The conversation id.
		/// </summary>
		public Guid ConversationId { get; set; }

		/// <summary>
		/// The action model.
		/// </summary>
		public IChatAction Action { get; set; }

		/// <summary>
		/// The content of the action.
		/// </summary>
		public string Content { get; set; }
	}
}