using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Chat.Application.Commands
{
	public class MarkConversationAsReadCommand : Command<Unit>
	{
		public Guid ConversationId { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public MarkConversationAsReadCommand(Guid conversationId, DateTime timeStamp)
		{
			ConversationId = conversationId;
			TimeStamp = timeStamp;
		}

		public override bool IsValid()
		{
			return ConversationId != Guid.Empty && TimeStamp > DateTime.MinValue;
		}
	}
}