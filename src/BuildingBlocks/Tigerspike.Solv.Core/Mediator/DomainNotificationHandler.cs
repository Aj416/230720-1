using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tigerspike.Solv.Core.Notifications
{
	public class DomainNotificationHandler : IDomainNotificationHandler
	{
		private List<DomainNotification> _notifications;

		public DomainNotificationHandler() => _notifications = new List<DomainNotification>();

		public virtual IEnumerable<DomainNotification> GetNotifications() => _notifications;

		public virtual bool HasNotifications() => GetNotifications().Any();

		public void IDispose() => _notifications = new List<DomainNotification>();

		public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
		{
			_notifications.Add(notification);
			return Task.CompletedTask;
		}
	}
}