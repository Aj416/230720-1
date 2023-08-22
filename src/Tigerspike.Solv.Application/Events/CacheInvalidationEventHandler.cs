using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Events.User;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class CacheInvalidationEventHandler :
		INotificationHandler<TicketReservationFailedEvent>,
		INotificationHandler<UserBlockedEvent>,
		INotificationHandler<UserUnblockedEvent>,
		INotificationHandler<AdvocateBrandsRemovedEvent>,
		INotificationHandler<AdvocateBrandsAssignedEvent>,
		INotificationHandler<AdvocateInductionCompletedEvent>,
		INotificationHandler<AdvocateBrandEnabledEvent>,
		INotificationHandler<AdvocateBrandDisabledEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly ILogger<CacheInvalidationEventHandler> _logger;

		public CacheInvalidationEventHandler(IMediatorHandler mediator, IRedisClientsManager redisClientsManager, ICachedBrandRepository cachedBrandRepository, ILogger<CacheInvalidationEventHandler> logger)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_redisClientsManager = redisClientsManager;
			_cachedBrandRepository = cachedBrandRepository;
			_logger = logger;
		}

		public async Task Handle(UserBlockedEvent notification, CancellationToken cancellationToken)
		{
			await Invalidate(CacheKeys.UserEnabledKey(notification.UserId));
			await Invalidate(CacheKeys.AdvocateBrandsKey(notification.UserId));
		}
		public Task Handle(UserUnblockedEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.UserEnabledKey(notification.UserId));
		public Task Handle(AdvocateBrandsRemovedEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.AdvocateBrandsKey(notification.AdvocateId));
		public Task Handle(AdvocateBrandsAssignedEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.AdvocateBrandsKey(notification.AdvocateId));
		public Task Handle(AdvocateInductionCompletedEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.AdvocateBrandsKey(notification.AdvocateId));
		public Task Handle(AdvocateBrandEnabledEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.AdvocateBrandsKey(notification.AdvocateId));
		public Task Handle(AdvocateBrandDisabledEvent notification, CancellationToken cancellationToken) => Invalidate(CacheKeys.AdvocateBrandsKey(notification.AdvocateId));
		public async Task Handle(TicketReservationFailedEvent notification, CancellationToken cancellationToken)
		{
			var availableTicketsCount = await _cachedBrandRepository.GetAvailableTickets(notification.AdvocateId, notification.Level);
			if (availableTicketsCount > 0)
			{
				// cache clearly needs to be invalidated
				_logger.LogDebug("Invalidating cache for ticket count for brands {@brandIds}", notification.BrandIds);
				var brandKeys = notification.BrandIds.Select(x => CacheKeys.AvailableTicketsKey(x, notification.Level)).ToArray();
				await Invalidate(brandKeys);
				await Invalidate(CacheKeys.AdvocateTouchedTicketsKey(notification.AdvocateId));

				foreach (var brandId in notification.BrandIds)
				{
					await _mediator.RaiseEvent(new BrandAvailableTicketCountChangedEvent(brandId, notification.Level));
				}
			}
		}

		private Task Invalidate(params string[] cacheKeys)
		{
			using var redisClient = _redisClientsManager.GetClient();
			foreach (var key in cacheKeys)
			{
				redisClient.Remove(key);
			}

			return Task.CompletedTask;
		}
	}
}
