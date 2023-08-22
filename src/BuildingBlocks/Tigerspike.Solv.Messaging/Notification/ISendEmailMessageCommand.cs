using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Notification
{
	public interface ISendEmailMessageCommand
	{
		string ReplyTo { get; }
		bool ReplyToTicket { get; }
		string MailTo { get; }
		string SenderName { get; }
		string Subject { get; }
		string Template { get; }
		Dictionary<string, object> Model { get; }
		IAttachment Attachment { get; }
		string Culture { get; }
	}
}