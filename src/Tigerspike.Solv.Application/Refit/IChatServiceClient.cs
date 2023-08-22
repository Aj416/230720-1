using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Tigerspike.Solv.Application.Models.Chat;

namespace Tigerspike.Solv.Application.Interfaces
{
	/// <summary>
	/// IChatApi interface.
	/// </summary>
	public interface IChatServiceClient
	{
		/// <summary>
		/// Gets all the messages for a given conversation.
		/// </summary>
		[Get("/{conversationId}/messages")]
		[Headers("Content-Type:application/json")]
		Task<List<MessageModel>> GetMessages([Query] Guid conversationId);

		/// <summary>
		/// Gets the conversations for the provided ids.
		/// </summary>
		[Get("/conversations")]
		[Headers("Content-Type:application/json")]
		Task<List<ConversationModel>> GetConversations([Query] string conversationIds);

		/// <summary>
		/// Gets the conversation for the provided id.
		/// </summary>
		[Get("/{conversationId}")]
		[Headers("Content-Type:application/json")]
		Task<ConversationModel> GetConversation([Query] Guid conversationId);

		/// <summary>
		/// Gets the transcript of the conversation for the provided id.
		/// </summary>
		[Get("/{conversationId}/transcript")]
		[Headers("Content-Type:application/json")]
		Task<string> GetTranscript([Query] Guid conversationId);

		/// <summary>
		/// Creates a conversation.
		/// </summary>
		[Post("/{conversationId}")]
		[Headers("Content-Type:application/json")]
		Task CreateConversation([Query] Guid conversationId, [Body] ConversationCreateModel conversation);

		/// <summary>
		/// Adds a message to a conversation.
		/// </summary>
		[Post("/{conversationId}/messages")]
		[Headers("Content-Type:application/json")]
		Task<MessageModel> AddMessage([Query] Guid conversationId, [Body] MessageAddModel message);

		/// <summary>
		/// Marks a conversation as read.
		/// </summary>
		[Patch("/{conversationId}/read")]
		[Headers("Content-Type:application/json")]
		Task MarkConversationAsRead([Query] Guid conversationId);

		/// <summary>
		/// Creates a conversation.
		/// </summary>
		[Post("/{conversationId}/messages/{messageId}/action")]
		[Headers("Content-Type:application/json")]
		Task<MessageModel> PostActionResponse([Query] Guid conversationId, [Query] Guid messageId, [Query] Guid userId,
			[Body] ActionRequestModel model);

		/// <summary>
		/// Finalize the active action on the conversation.
		/// </summary>
		[Patch("/{conversationId}/actions/finalize")]
		[Headers("Content-Type:application/json")]
		Task FinalizeActiveActions([Query] Guid conversationId);

		/// <summary>
		/// Post the solved question to the chat.
		/// </summary>
		[Post("/{conversationId}/actions/solved")]
		[Headers("Content-Type:application/json")]
		Task<MessageModel> AddIsTicketSolvedQuestion([Query] Guid conversationId, [Query] Guid advocateId);

		/// <summary>
		/// Inserts phrases to whitelist corresponding to a brand. To be moved to the brand service eventually.
		/// </summary>
		[Post("/brands/{brandId}/whitelist")]
		[Headers("Content-Type:application/json")]
		Task<string[]> AddWhitelistPhrases([Query] Guid brandId, [Body] string[] whitelistPhrases);

		/// <summary>
		/// Deletes phrases from whitelist corresponding to a brand. To be moved to the brand service eventually.
		/// </summary>
		[Delete("/brands/{brandId}/whitelist")]
		[Headers("Content-Type:application/json")]
		Task<string[]> RemoveWhitelistPhrases([Query] Guid brandId, [Body] string[] whitelistPhrases);
	}
}