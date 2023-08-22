using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.Redis;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Application.EventHandlers
{
	public class TicketHubEventHandler :
		INotificationHandler<AdvocateBrandEnabledEvent>,
		INotificationHandler<AdvocateBrandDisabledEvent>
	{
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly IHubContext<TicketHub> _ticketHub;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITimestampService _timestampService;

		public TicketHubEventHandler(
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
		public async Task Handle(AdvocateBrandEnabledEvent notification, CancellationToken cancellationToken)
		{
			using var client = _redisClientsManager.GetClient();
			var connections = client.AdvocateConnections(_timestampService, notification.AdvocateId).GetAll();
			client.BrandOnlineAdvocates(_timestampService, notification.BrandId).Add(notification.AdvocateId);
			foreach (var cid in connections)
			{
				await _ticketHub.Groups.AddToGroupAsync(cid, TicketHubConstants.GetBrandGroupName(notification.BrandId));
			}
		}

		public async Task Handle(AdvocateBrandDisabledEvent notification, CancellationToken cancellationToken)
		{
			using var client = _redisClientsManager.GetClient();
			var connections = client.AdvocateConnections(_timestampService, notification.AdvocateId).GetAll();
			client.BrandOnlineAdvocates(_timestampService, notification.BrandId).Remove(notification.AdvocateId);
			foreach (var cid in connections)
			{
				await _ticketHub.Groups.RemoveFromGroupAsync(cid, TicketHubConstants.GetBrandGroupName(notification.BrandId));
			}
		}

	}
}
