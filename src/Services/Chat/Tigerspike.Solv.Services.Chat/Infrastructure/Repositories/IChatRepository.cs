using System;
using System.Collections.Generic;
using Tigerspike.Solv.Chat.Domain;

namespace Tigerspike.Solv.Chat.Infrastructure.Repositories
{
	public interface IChatRepository
	{
		/// <summary>
		/// Gets the chat message.
		/// </summary>
		/// <param name="conversationId">The conversation identifier.</param>
		/// <param name="messageId">The message identifier.</param>
		/// <returns>The chat message.</returns>
		Message GetMessage(Guid conversationId, Guid messageId);

		/// <summary>
		/// Gets the chat messages of a conversation.
		/// </summary>
		/// <param name="conversationId">The conversation identifier.</param>
		/// <returns>The chat messages of the conversation.</returns>
		List<Message> GetMessages(Guid conversationId);

		/// <summary>
		/// Gets the all action messages of a conversation
		/// </summary>
		/// <param name="conversationId">The conversation identifier.</param>
		List<Message> GetActions(Guid conversationId);

		/// <summary>
		/// Deletes the item based on the conversation and id.
		/// </summary>
		/// <param name="conversationId">The conversation identifier.</param>
		/// <param name="messageId">The item range key.</param>
		/// <returns></returns>
		void DeleteMessage(Guid conversationId, string messageId);

		/// <summary>
		/// Adds or updates the item in the store.
		/// </summary>
		/// <param name="message">The message to add</param>
		/// <returns>The created or updated message.</returns>
		Message AddOrUpdateMessage(Message message);

		/// <summary>
		/// Add or update the conversation.
		/// </summary>
		/// <param name="conversation">The conversation to update</param>
		/// <returns>The updated conversation</returns>
		Conversation AddOrUpdateConversation(Conversation conversation);

		/// <summary>
		/// Get all the conversations that match the list of ids passed.
		/// </summary>
		/// <param name="conversationIds">The list of ids of the conversations</param>
		/// <returns>A list of conversations object</returns>
		List<Conversation> GetConversations(params Guid[] ids);

		/// <summary>
		/// Gets the conversation for the specified id.
		/// </summary>
		/// <param name="id">The conversation id.</param>
		/// <returns>The conversation.</returns>
		Conversation GetConversation(Guid id);
	}
}