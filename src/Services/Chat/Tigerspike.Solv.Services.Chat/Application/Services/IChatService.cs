using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Services.Chat.Application.Models;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Chat.Application.Services
{
	public interface IChatService
	{
		/// <summary>
		/// Gets the messages for a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>The messages for the ticket</returns>
		Task<List<MessageModel>> GetMessages(Guid ticketId);

		/// <summary>
		/// Gets messages for a ticket from desired perspective
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		/// <param name="perspective">The user's perspective</param>
		/// <returns>The messages for the ticket relevant to specified perspective</returns>
		Task<IEnumerable<MessageModel>> GetMessages(Guid ticketId, UserType perspective);

		/// <summary>
		/// Deletes a messages.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		/// <param name="authorId">The author id.</param>
		/// <param name="messageId">The message id.</param>
		/// <param name="timestamp">The create date in epoch time.</param>
		Task DeleteMessage(Guid conversationId, Guid authorId, Guid messageId, long timestamp);

		/// <summary>
		/// Posts a new message.
		/// </summary>
		/// <param name="message">The new chat message.</param>
		/// <returns>The message model.</returns>
		Task<MessageModel> AddMessage(Guid conversationId, MessageCreateModel model);

		/// <summary>
		/// Post a new message as solver
		/// </summary>
		/// <param name="conversationId">The new chat message.</param>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="isSuperSolver">Is advocate a suepr solver or not</param>
		/// <param name="response">The reposnse text</param>
		Task<MessageModel> AddSolverResponse(Guid conversationId, Guid advocateId, bool isSuperSolver, string response);

		/// <summary>
		/// Adds a chat question asking customer whether the ticket is solved now
		/// </summary>
		/// <param name="conversationId">Ticket/conversation id</param>
		/// <param name="advocateId">The advocate/solver id</param>
		Task<MessageModel> AddIsTicketSolvedQuestion(Guid conversationId, Guid advocateId);

		/// <summary>
		/// Adds a chat request for support rating (CSAT)
		/// </summary>
		/// <param name="conversationId">Ticket/conversation id</param>
		/// <param name="advocateId">The advocate/solver id</param>
		Task<MessageModel> AddCsatAction(Guid conversationId, Guid advocateId);

		/// <summary>
		/// Adds a NPS request form
		/// </summary>
		/// <param name="conversationId">Ticket/conversation id</param>
		/// <param name="advocateId">The advocate/solver id</param>
		/// <param name="brandName">The name of the brand</param>
		Task<MessageModel> AddNpsAction(Guid conversationId, Guid advocateId, string brandName);

		/// <summary>
		/// Adds a new conversation.
		/// </summary>
		/// <param name="conversation">The new conversation</param>
		/// <returns>The id of the new conversation</returns>
		Guid AddConversation(ConversationCreateModel model);

		/// <summary>
		/// Mark the conversation as read (last read date to be the current date)
		/// </summary>
		/// <param name="conversationId">The id of the conversation</param>
		/// <returns>A task with void return type</returns>
		Task MarkConversationAsRead(Guid conversationId);

		/// <summary>
		/// Get all the conversations that matched the list of ids passed.
		/// </summary>
		/// <param name="conversationIds">The list of ids of the conversations</param>
		/// <returns>A list of conversations models.</returns>
		List<ConversationModel> GetConversations(params Guid[] conversationIds);

		/// <summary>
		/// Gets the conversation by id.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		/// <returns>The conversation model.</returns>
		ConversationModel GetConversation(Guid conversationId);

		/// <summary>
		/// Store an response to the chat action
		/// </summary>
		/// <param name="conversationId">Conversation/ticket id</param>
		/// <param name="messageId">Message/action id</param>
		/// <param name="model">Response model</param>
		/// <param name="userId">User Id</param>
		/// <returns>Updated message</returns>
		Task<MessageModel> PostActionResponse(Guid conversationId, Guid messageId, ActionRequestModel model, Guid userId);

		/// <summary>
		/// Marks all active actions in chat as finalized
		/// </summary>
		/// <param name="conversationId">The conversation/ticket id</param>
		Task FinalizeActiveActions(Guid conversationId);

		/// <summary>
		/// Gets the chat transcript for a ticket.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		/// <returns>The chat transcript text.</returns>
		Task<string> GetTranscript(Guid conversationId);
	}
}