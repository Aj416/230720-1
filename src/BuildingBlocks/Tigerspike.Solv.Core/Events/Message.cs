using MediatR;

namespace Tigerspike.Solv.Core.Events
{
	public abstract class Message : INotification
	{
		public string MessageType { get; protected set; }

		protected Message()
		{
			MessageType = GetType().Name;
		}
	}
}