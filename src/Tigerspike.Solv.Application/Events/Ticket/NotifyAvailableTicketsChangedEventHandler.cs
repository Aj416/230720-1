using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using static Tigerspike.Solv.Application.SignalR.TicketHubConstants;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	public class NotifyAvailableTicketsChangedEventHandler :
		INotificationHandler<BrandAvailableTicketCountChangedEvent>,
		INotificationHandler<TicketReservedEvent>,
		INotificationHandler<TicketAcceptedEvent>,
		INotificationHandler<TicketRejectedEvent>,
		INotificationHandler<TicketAbandonedEvent>,
		INotificationHandler<TicketEscalatedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<AdvocateBrandsRemovedEvent>,
		INotificationHandler<AdvocateBrandEnabledEvent>,
		INotificationHandler<AdvocateBrandDisabledEvent>
	{
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly IHubContext<TicketHub> _ticketHub;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITimestampService _timestampService;

		public NotifyAvailableTicketsChangedEventHandler(
			IHubContext<TicketHub> ticketHub,
			IRedisClientsManager redisClientsManager,
			ITimestampService timestampService,
			ICachedBrandRepository cachedBrandRepository)
		{
			_cachedBrandRepository = cachedBrandRepository;
			_ticketHub = ticketHub;
			_redisClientsManager = redisClientsManager;
			_timestampService = timestampService;
		}

		public async Task Handle(TicketRejectedEvent notification, CancellationToken cancellationToken)
		{
			_cachedBrandRepository.TouchTicket(notification.TicketId, notification.AdvocateId);
			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId); // remove ticket from current queue
			_cachedBrandRepository.AddTicket(notification.TicketId, notification.BrandId, notification.Level); // add ticket to new queue
			await NotifyAvailableTicketChanged(notification.BrandId);
		}

		public async Task Handle(TicketAbandonedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AutoAbandoned)
			{
				// mark ticket as available again for this advocate
				_cachedBrandRepository.UntouchTicket(notification.TicketId, notification.AdvocateId);
			}
			else
			{
				// mark ticket as unavailable for this advocate
				_cachedBrandRepository.TouchTicket(notification.TicketId, notification.AdvocateId);
			}

			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId); // remove ticket from current queue
			_cachedBrandRepository.AddTicket(notification.TicketId, notification.BrandId, notification.Level); // add ticket to new queue
			await NotifyAvailableTicketChanged(notification.BrandId);
		}
		public async Task Handle(TicketReservedEvent notification, CancellationToken cancellationToken)
		{
			_cachedBrandRepository.TouchTicket(notification.TicketId, notification.AdvocateId);
			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId); // remove ticket from current queue
			await NotifyAvailableTicketChanged(notification.BrandId);
		}

		public async Task Handle(TicketEscalatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AdvocateId.HasValue)
			{
				_cachedBrandRepository.TouchTicket(notification.TicketId, notification.AdvocateId.Value);
			}

			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId); // remove ticket from current queue
			_cachedBrandRepository.AddTicket(notification.TicketId, notification.BrandId, notification.Level); // add ticket to new queue
			await NotifyAvailableTicketChanged(notification.BrandId);
		}

		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			_cachedBrandRepository.TouchTicket(notification.TicketId, notification.AdvocateId);
			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId); // remove ticket from current queue
			await NotifyAvailableTicketChanged(notification.BrandId);
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			_cachedBrandRepository.RemoveTicket(notification.TicketId, notification.BrandId);
			await NotifyAvailableTicketChanged(notification.BrandId);
		}

		public Task Handle(AdvocateBrandsRemovedEvent notification, CancellationToken cancellationToken) =>	_ticketHub.Clients.User(notification.AdvocateId.ToString()).SendAsync(BRAND_AVAILABLE_TICKETS_UPDATED);
		public Task Handle(AdvocateBrandEnabledEvent notification, CancellationToken cancellationToken) =>	_ticketHub.Clients.User(notification.AdvocateId.ToString()).SendAsync(BRAND_AVAILABLE_TICKETS_UPDATED);
		public Task Handle(AdvocateBrandDisabledEvent notification, CancellationToken cancellationToken) => _ticketHub.Clients.User(notification.AdvocateId.ToString()).SendAsync(BRAND_AVAILABLE_TICKETS_UPDATED);
		public Task Handle(BrandAvailableTicketCountChangedEvent notification, CancellationToken cancellationToken) => NotifyAvailableTicketChanged(notification.BrandId);
		private Task NotifyAvailableTicketChanged(Guid brandId) => _ticketHub.Clients.Group(GetBrandGroupName(brandId)).SendAsync(BRAND_AVAILABLE_TICKETS_UPDATED);
	}
}
