using System.Collections.Generic;
using System.Dynamic;

namespace Tigerspike.Solv.Messaging.Notification
{
	public interface ISendMessengerMessageCommand
	{
		string ConversationId { get; }
		string Text { get; }
	}
}