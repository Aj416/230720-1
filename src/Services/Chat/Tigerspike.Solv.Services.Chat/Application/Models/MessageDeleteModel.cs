using System;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class MessageDeleteModel
	{
		public Guid Id { get; set; }

		public Guid ConversationId { get; set; }

		public MessageDeleteModel(Guid id, Guid conversationId)
		=> (Id, ConversationId) = (id, conversationId);

	}
}