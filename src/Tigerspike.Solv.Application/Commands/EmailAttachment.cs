using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Commands
{
	public class EmailAttachment : IAttachment
	{
		public string Filename { get; set; }
		public string Content { get; set; }
		public string ContentType { get; set; }
	}
}