using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Chat.Domain;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;

namespace Tigerspike.Solv.Services.Chat.Infrastructure.Repositories
{
	public class ChatRepository : IChatRepository
	{
		private readonly IPocoDynamo _db;

		public ChatRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc />
		public void DeleteMessage(Guid conversationId, string messageId) => _db.DeleteItem<Message>(new DynamoId(conversationId.ToString(), messageId));

		/// <inheritdoc />
		public Message AddOrUpdateMessage(Message message) => _db.PutItem(message);

		/// <inheritdoc />
		public Conversation AddOrUpdateConversation(Conversation conversation) => _db.PutItem(conversation);

		/// <inheritdoc />
		public Message GetMessage(Guid conversationId, Guid messageId)
		{
			return _db
				.FromQuery<Message>()
				.KeyCondition(m => m.ConversationId == conversationId.ToString())
				.Filter(m => m.Id == messageId.ToString())
				.Exec().FirstOrDefault();
		}

		/// <inheritdoc />
		public List<Message> GetMessages(Guid conversationId)
		{
			return _db
				.FromQuery<Message>().KeyCondition(m => m.ConversationId == conversationId.ToString())
				.Exec().ToList();
		}

		/// <inheritdoc />
		public List<Message> GetActions(Guid conversationId)
		{
			return _db
				.FromQuery<Message>()
				.KeyCondition(m => m.ConversationId == conversationId.ToString())
				.Filter(m => m.Type == (int)MessageType.Action)
				.Exec()
				.ToList();
		}

		/// <inheritdoc />
		public List<Conversation> GetConversations(params Guid[] ids)
		{
			var idsAsString = ids.Select(id => id.ToString()).ToArray();
			return _db.GetItems<Conversation>(idsAsString);
		}

		/// <inheritdoc />
		public Conversation GetConversation(Guid id)
		{
			return _db.GetItem<Conversation>(id.ToString());
		}
	}
}