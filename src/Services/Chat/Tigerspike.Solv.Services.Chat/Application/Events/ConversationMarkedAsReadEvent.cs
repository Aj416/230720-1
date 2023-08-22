using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class ConversationMarkedAsReadEvent: Event
	{
		public Guid ConversationId { get; private set; }

		public ConversationMarkedAsReadEvent(Guid conversationId)
		{
			ConversationId = conversationId;
		}
	}
}