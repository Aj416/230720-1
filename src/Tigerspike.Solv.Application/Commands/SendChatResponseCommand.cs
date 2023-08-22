using System;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Commands
{
	public class SendChatResponseCommand : ISendAutoChatResponseCommand, IScheduledJob
	{
		public string JobId => $"{nameof(SendChatResponseCommand)}-{ResponseId}-{TicketId}";

		public Guid ResponseId { get; private set; }
		public Guid TicketId { get; private set; }
		public Guid? AuthorId { get; private set; }
		public int? ResponseType { get; private set; }
		public int SenderType { get; private set; }
		public string Content { get; private set; }
		public string RelevantTo { get; private set; }

		public Guid? ActionId {get; private set; }

		/// <summary>
		/// The empty constructor for the bus activator
		/// </summary>
		public SendChatResponseCommand()
		{

		}

		/// <summary>
		/// The short constructor - for message cancelling
		/// </summary>
		public SendChatResponseCommand(Guid responseId, Guid ticketId)
		{
			ResponseId = responseId;
			TicketId = ticketId;
		}

		/// <summary>
		/// The constructor.
		/// </summary>
		public SendChatResponseCommand(Guid responseId, Guid ticketId, Guid? authorId, int? responseType, int senderType, string content, string relevantTo, Guid? actionId)
		{
			ResponseId = responseId;
			TicketId = ticketId;
			AuthorId = authorId;
			ResponseType = responseType;
			SenderType = senderType;
			Content = content;
			RelevantTo = relevantTo;
			ActionId = actionId;
		}

	}
}