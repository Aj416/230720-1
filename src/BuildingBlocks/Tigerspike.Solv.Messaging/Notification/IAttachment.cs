namespace Tigerspike.Solv.Messaging.Notification
{
	public interface IAttachment
	{
		string Filename { get; set; }

		string Content { get; set; }

		string ContentType { get; set; }
	}
}