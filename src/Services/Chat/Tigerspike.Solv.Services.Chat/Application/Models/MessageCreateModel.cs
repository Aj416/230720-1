using System;
using System.Collections.Generic;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{

	public class MessageCreateModel
	{
		public Guid? AuthorId { get; set; }

		public UserType SenderType { get; set; }

		public IEnumerable<UserType> RelevantTo { get; set; }

		public string Message { get; set; }

		public Guid ClientMessageId { get; set; }

		public MessageType MessageType { get;set; }

		public ActionModel Action { get; set; }
	}
}