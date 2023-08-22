using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Services.Chat.Application.Models;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class ActionFinalizedEvent : Event
	{
		public Guid ConversationId { get; private set; }
		public ActionModel Action { get; private set; }
		public string Content { get; private set; }

		public ActionFinalizedEvent(Guid conversationId, ActionModel action, string content) =>
			(ConversationId, Action, Content) = (conversationId, action, content);
	}
}
