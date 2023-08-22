using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Chat.Application.Commands
{
	public class UpdateConversationLastMessageCommand : Command
	{
		public Guid ConversationId { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public UpdateConversationLastMessageCommand(Guid conversationId, DateTime timeStamp)
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