using System;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Services.Chat.Application.IntegrationEvents
{
	public class ChatMessageDeletedEvent : IChatMessageDeletedEvent
	{
		/// <inheritdoc />
		public Guid ConversationId { get; set; }

		/// <inheritdoc />
		public Guid MessageId { get; set; }

		/// <inheritdoc />
		public Guid CustomerId { get; set; }

		/// <inheritdoc />
		public Guid? AdvocateId { get; set; }
	}
}