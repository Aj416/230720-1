using System;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class MessageAddModel
	{
		public string Message { get; set; }

		public Guid ClientMessageId { get; set; }

		public Guid? AuthorId { get; set; }

		public int SenderType { get; set; }

		public int? MessageType { get; set; }

		public int[] RelevantTo { get; set; }
	}

}