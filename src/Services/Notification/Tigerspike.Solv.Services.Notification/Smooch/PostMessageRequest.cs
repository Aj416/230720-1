namespace Tigerspike.Solv.Services.Notification.Smooch
{
	public class PostMessageRequest
	{
		public PostMessageAuthor Author { get; }
		public PostMessageContent Content { get; }

		public PostMessageRequest(PostMessageAuthor author, PostMessageContent content)
		{
			Author = author;
			Content = content;
		}
	}

	public class PostMessageContent
	{
		public string Type { get; protected set; }
		public string Text { get; protected set; }

		public PostMessageContent(string type, string text)
		{
			Type = type;
			Text = text;
		}

		public PostMessageContent(string text)
		{
			Type = "text";
			Text = text;
		}
	}

	public class PostMessageAuthor
	{
		public string Type { get; protected set; }

		public PostMessageAuthor(string type)
		{
			Type = type;
		}

		public PostMessageAuthor()
		{
			Type = "business";
		}
	}
}