using System;

namespace Tigerspike.Solv.Services.Chat
{
	public static class CacheKeys
	{
		public static readonly Func<Guid, string> ConversationKey = conversationId => $"conversation:{conversationId}:3789";
	}
}