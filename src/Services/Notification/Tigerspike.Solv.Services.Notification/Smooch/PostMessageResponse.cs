using System;

namespace Tigerspike.Solv.Services.Notification.Smooch
{
	public class PostMessageResponse
	{
		public PostedMessage[] Messages { get; set; }
	}

	public class PostedMessage
	{
		public string Id { get; set; }
	}
}