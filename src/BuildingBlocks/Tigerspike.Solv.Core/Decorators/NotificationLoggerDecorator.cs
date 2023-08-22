using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Core.Decorators
{
	public class NotificationLoggerDecorator<TNotification> : INotificationHandler<TNotification>
		where TNotification : INotification
	{
		private readonly INotificationHandler<TNotification> _handler;
		private readonly ILogger<NotificationLoggerDecorator<TNotification>> _logger;

		public NotificationLoggerDecorator(INotificationHandler<TNotification> handler, ILogger<NotificationLoggerDecorator<TNotification>> logger)
		{
			_handler = handler;
			_logger = logger;
		}

		public async Task Handle(TNotification notification, CancellationToken cancellationToken)
		{
			var notificationType = notification.GetType().Name;
			var handlerType = _handler.GetType().Name;
			var operationId = Guid.NewGuid().ToString().Truncate(6);

			_logger.LogDebug("Handling {notification} by {handler} started ({operationId}) ({@request})", notificationType, handlerType, operationId, notification);
			await _handler.Handle(notification, cancellationToken);
			_logger.LogDebug("Handling {notification} by {handler} finished ({operationId}) ({@request})", notificationType, handlerType, operationId, notification);
		}

	}
}
