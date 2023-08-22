using System;

namespace Tigerspike.Solv.Messaging.Chat
{
	public interface ISendAutoChatResponseCommand
	{
		public string JobId { get; }

		public Guid ResponseId { get; }

		public Guid TicketId { get; }

		public Guid? AuthorId { get; }

		public int? ResponseType { get; }

		public int SenderType { get; }

		public string Content { get; }

		public string RelevantTo { get; }

		public Guid? ActionId {get; }
	}
}