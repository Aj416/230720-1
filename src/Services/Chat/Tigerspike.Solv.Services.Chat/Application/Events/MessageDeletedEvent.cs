using System;
using Tigerspike.Solv.Core.Events;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class MessageDeletedEvent : Event
	{
		public Guid ConversationId { get; private set; }

		public Guid CustomerId { get; private set; }

		public Guid? AdvocateId { get; private set; }

		public Guid MessageId { get; private set; }

		public MessageDeletedEvent(Guid conversationId, Guid customerId, Guid? advocateId, Guid messageId)
		{
			ConversationId = conversationId;
			CustomerId = customerId;
			AdvocateId = advocateId;
			MessageId = messageId;
		}

	}
}
