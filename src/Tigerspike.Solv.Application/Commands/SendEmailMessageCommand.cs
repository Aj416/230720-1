using System.Collections.Generic;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Commands
{
	public class SendEmailMessageCommand : ISendEmailMessageCommand
	{
		public string ReplyTo { get; set; }
		public bool ReplyToTicket { get; set; }
		public string MailTo { get; set; }
		public string SenderName { get; set; }
		public string Subject { get; set; }
		public string Template { get; set; }
		public Dictionary<string, object> Model { get; set; }
		public IAttachment Attachment { get; set; }
		public string Culture { get; set; }
	}
}