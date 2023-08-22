using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Services
{
	public interface IChatService : IChatServiceClient
	{
		/// <summary>
		/// Add system messages to the chat.
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="chatType"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		public Task AddSystemMessage(Guid ticketId, MessageType chatType, string msgText,
			params string[] msgTextArgs) => AddChatMessage(ticketId, Guid.Empty, chatType, UserType.System, null,
			msgText, msgTextArgs);

		/// <summary>
		/// Add system messages to the chat.
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="chatType"></param>
		/// <param name="relevantTo"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		public Task AddSystemMessage(Guid ticketId, MessageType chatType, IEnumerable<UserType> relevantTo,
			string msgText,
			params string[] msgTextArgs) => AddChatMessage(ticketId, Guid.Empty, chatType, UserType.System, relevantTo,
			msgText, msgTextArgs);

		/// <summary>
		/// Adds a message to the chat
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="authorId"></param>
		/// <param name="chatType"></param>
		/// <param name="senderType"></param>
		/// <param name="relevantTo"></param>
		/// <param name="msgText"></param>
		/// <param name="msgTextArgs"></param>
		/// <returns></returns>
		public async Task AddChatMessage(Guid ticketId, Guid? authorId, MessageType chatType, UserType senderType,
			IEnumerable<UserType> relevantTo, string msgText,
			params string[] msgTextArgs) =>
			await this.AddMessage(ticketId, new MessageAddModel
			{
				AuthorId = authorId,
				SenderType = (int)senderType,
				RelevantTo = relevantTo?.Select(x => (int)x).ToArray(),
				Message = string.Format(msgText, msgTextArgs),
				MessageType = (int?)chatType
			});
	}
}