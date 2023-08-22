using System;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public interface ICachedConversationRepository
	{
		/// <summary>
		/// Gets the conversation for the specified id.
		/// </summary>
		/// <param name="id">The conversation id.</param>
		/// <returns>The conversation.</returns>
		Conversation GetConversation(Guid id);

		/// <summary>
		/// Remove the cached conversation data.
		/// </summary>
		void InvalidateConversation(Guid conversationId);
	}
}
