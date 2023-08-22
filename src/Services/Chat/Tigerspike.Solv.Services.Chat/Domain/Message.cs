using System;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Chat.Enums;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Chat.Domain
{
	public class Message
	{
		[HashKey]
		public string ConversationId { get; set; }

		[RangeKey]
		public string MessageId { get; set; }

		private DateTime createdDate;

		public DateTime CreatedDate
		{
			get => createdDate;
			// Round the ticks to the first thousand
			set => createdDate = value.AddTicks(-(value.Ticks % 10000));
		}

		public int SenderType { get; set; }

		public int[] RelevantTo { get; set; }

		public string AuthorId { get; set; }

		public string Content { get; set; }

		public MessageFile File { get; set; }

		/// <summary>
		/// Additional action contained in message
		/// </summary>
		public Action Action { get; set; }

		public int Type { get; set; }

		public string Id { get; set; } // Generated id for a message -- read-only

		public Message() { }

		public Message(string conversationId, string authorId, UserType senderType, string content, MessageType type)
		{
			if (string.IsNullOrEmpty(conversationId))
			{
				throw new ArgumentNullException(nameof(conversationId));
			}

			ConversationId = conversationId;
			AuthorId = authorId;
			SenderType = (int)senderType;
			Content = content;
			Type = (int)type;
			CreatedDate = DateTime.UtcNow;
			MessageId = GenerateMessageId(CreatedDate, AuthorId);
			Id = Guid.NewGuid().ToString();
		}

		public Message(string conversationId, string authorId, string content)
		{
			if (string.IsNullOrEmpty(conversationId))
			{
				throw new ArgumentNullException(nameof(conversationId));
			}

			ConversationId = conversationId;
			AuthorId = authorId;
			SenderType = (int)UserType.Customer;
			Content = content;
			Type = (int)MessageType.Message;
			CreatedDate = DateTime.UtcNow;
			MessageId = GenerateMessageId(CreatedDate, AuthorId);
			Id = Guid.NewGuid().ToString();
		}

		public static string GenerateMessageId(DateTime createdDate, string authorId)
		{
			var createdDateTimeOffSet = new DateTimeOffset(createdDate);
			return GenerateMessageId(createdDateTimeOffSet.ToUnixTimeMilliseconds(), authorId);
		}

		public static string GenerateMessageId(long createdDateTimeStamp, string authorId) => $"{createdDateTimeStamp}_{authorId}";
	}
}