namespace Tigerspike.Solv.Application.SignalR
{
	public static class ChatHubConstants
	{
		public const string MESSAGE_ADDED = "CHAT_MESSAGE_ADDED";
		public const string MESSAGE_UPDATED = "CHAT_MESSAGE_UPDATED";
		public const string MESSAGE_DELETED = "CHAT_MESSAGE_DELETED";
		public const string MESSAGE_USERTYPING = "CHAT_MESSAGE_USERTYPING";
		public const string MESSAGE_USERSTOPPEDYPING = "CHAT_MESSAGE_USERSTOPPEDYPING";
		public const string CUSTOMER_ONLINE = "CHAT_CUSTOMER_ONLINE";
		public const string CUSTOMER_OFFLINE = "CHAT_CUSTOMER_OFFLINE";
		public const string TICKET_STATUS_CHANGED = nameof(TICKET_STATUS_CHANGED);
	}
}