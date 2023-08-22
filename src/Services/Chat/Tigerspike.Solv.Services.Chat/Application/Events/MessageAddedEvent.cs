using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Services.Chat.Application.Models;

namespace Tigerspike.Solv.Services.Chat.Application.Events
{
	public class MessageAddedEvent : Event
	{
		public Guid ConversationId { get; private set; }

		public Guid CustomerId { get; private set; }

		public Guid? AdvocateId { get; private set; }

		public Guid BrandId { get; private set; }

		public MessageModel Message { get; private set; }

		public MessageAddedEvent(Guid conversationId, Guid customerId, Guid? advocateId, Guid brandId, MessageModel message, DateTime timestamp)
		{
			ConversationId = conversationId;
			CustomerId = customerId;
			AdvocateId = advocateId;
			BrandId = brandId;
			Message = message;
			Timestamp = timestamp;
		}
	}
}
