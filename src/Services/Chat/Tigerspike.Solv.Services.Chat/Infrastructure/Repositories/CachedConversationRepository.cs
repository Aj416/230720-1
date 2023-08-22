using System;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Redis;
using Tigerspike.Solv.Chat.Domain;
using static Tigerspike.Solv.Services.Chat.CacheKeys;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public class CachedConversationRepository : ICachedConversationRepository
	{
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly IChatRepository _chatRepository;
		private static readonly TimeSpan _defaultTtl = TimeSpan.FromHours(1);

		public CachedConversationRepository(IRedisClientsManager redisClientsManager, IChatRepository chatRepository)
		{
			_redisClientsManager = redisClientsManager;
			_chatRepository = chatRepository;
		}

		/// <inheritdoc />
		public Conversation GetConversation(Guid id)
		{
			using var client = _redisClientsManager.GetClient();
			var key = ConversationKey(id);

			if (client.ContainsKey(key))
			{
				return client.Get<Conversation>(key);
			}

			var value = _chatRepository.GetConversation(id);

			return value != null ? client.Set(key, value, expireIn: _defaultTtl) : null;
		}

		/// <inheritdoc/>
		public void InvalidateConversation(Guid conversationId)
		{
			using var client = _redisClientsManager.GetClient();

			// Delete key for this conversation
			client.Remove(ConversationKey(conversationId));
		}
	}
}
