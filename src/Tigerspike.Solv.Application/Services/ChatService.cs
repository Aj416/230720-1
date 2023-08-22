using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Chat;

namespace Tigerspike.Solv.Application.Services
{
	public class ChatService : IChatService
	{
		private readonly IChatServiceClient _chatServiceClient;

		/// <summary>
		/// ChatService constructor.
		/// </summary>
		public ChatService(
			IChatServiceClient chatServiceClient)
		{
			_chatServiceClient = chatServiceClient;
		}

		/// <inheritdoc />
		public Task<List<MessageModel>> GetMessages(Guid conversationId)
		{
			return _chatServiceClient.GetMessages(conversationId);
		}

		/// <inheritdoc />
		public Task<List<ConversationModel>> GetConversations(string conversationIds)
		{
			return _chatServiceClient.GetConversations(conversationIds);
		}

		/// <inheritdoc />
		public Task<ConversationModel> GetConversation(Guid conversationId)
		{
			return _chatServiceClient.GetConversation(conversationId);
		}

		public Task<string> GetTranscript(Guid conversationId)
		{
			return _chatServiceClient.GetTranscript(conversationId);
		}

		/// <inheritdoc />
		public Task CreateConversation(Guid conversationId, ConversationCreateModel conversation)
		{
			return _chatServiceClient.CreateConversation(conversationId, conversation);
		}

		/// <inheritdoc />
		public Task<MessageModel> AddMessage(Guid conversationId, MessageAddModel message)
		{
			return _chatServiceClient.AddMessage(conversationId, message);
		}

		/// <inheritdoc />
		public Task MarkConversationAsRead(Guid conversationId)
		{
			return _chatServiceClient.MarkConversationAsRead(conversationId);
		}

		/// <inheritdoc />
		public Task<MessageModel> PostActionResponse(Guid conversationId, Guid messageId, Guid userId,
			ActionRequestModel model)
		{
			return _chatServiceClient.PostActionResponse(conversationId, messageId, userId, model);
		}

		/// <inheritdoc />
		public Task<MessageModel> AddIsTicketSolvedQuestion(Guid conversationId, Guid advocateId)
		{
			return _chatServiceClient.AddIsTicketSolvedQuestion(conversationId, advocateId);
		}

		/// <inheritdoc />
		public Task FinalizeActiveActions(Guid conversationId)
		{
			return _chatServiceClient.FinalizeActiveActions(conversationId);
		}

		/// <inheritdoc />
		public Task<string[]> AddWhitelistPhrases(Guid brandId, string[] whitelistPhrases)
		{
			return _chatServiceClient.AddWhitelistPhrases(brandId, whitelistPhrases);
		}

		/// <inheritdoc />
		public Task<string[]> RemoveWhitelistPhrases(Guid brandId, string[] whitelistPhrases)
		{
			return _chatServiceClient.RemoveWhitelistPhrases(brandId, whitelistPhrases);
		}
	}
}