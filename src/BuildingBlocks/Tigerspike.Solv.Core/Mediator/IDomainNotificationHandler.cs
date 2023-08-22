using System.Collections.Generic;
using MediatR;

namespace Tigerspike.Solv.Core.Notifications
{
	public interface IDomainNotificationHandler : INotificationHandler<DomainNotification>
	{
		IEnumerable<DomainNotification> GetNotifications();

		bool HasNotifications();
	}
}
