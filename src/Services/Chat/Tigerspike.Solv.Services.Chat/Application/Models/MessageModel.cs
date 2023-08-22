using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tigerspike.Solv.Services.Chat.Application.Models
{
	public class MessageModel
	{
		public Guid Id { get; set; }

		public Guid ConversationId { get; set; }

		public string ThreadId { get; set; }

		public Guid AuthorId { get; set; }

		public int Type { get; set; }

		public DateTime CreatedDate { get; set; }

		public string Message { get; set; }

		public int MessageFileTypeId { get; set; }

		public string Path { get; set; }

		public string FileName { get; set; }

		public string MimeType { get; set; }

		public string UserFirstName { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Guid ClientMessageId { get; set; }

		public int SenderType { get; set; }

		public List<int> RelevantTo { get; set; }

		public ActionModel Action { get; set; }
	}
}