using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Services.Chat.Application.Models;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class MessageUpdatedEvent : Event
	{
		public Guid ConversationId { get; private set; }

		public Guid CustomerId { get; private set; }

		public Guid? AdvocateId { get; private set; }

		public MessageModel Message { get; private set; }

		public MessageUpdatedEvent(Guid conversationId, Guid customerId, Guid? advocateId, MessageModel message)
		{
			ConversationId = conversationId;
			CustomerId = customerId;
			AdvocateId = advocateId;
			Message = message;
		}
	}
}
