using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using static Tigerspike.Solv.Application.SignalR.TicketHubConstants;

namespace Tigerspike.Solv.Application.SignalR
{
	/// <summary>
	/// Ticket SignalR Hub, to push messages related to tickets in general
	/// </summary>
	[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver + "," + SolvRoles.Client)]
	public class TicketHub : Hub
	{
		private readonly IBrandService _brandService;
		private readonly IAdvocateService _advocateService;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ICachedBrandRepository _cachedBrandRepository;
		private readonly ITimestampService _timestampService;

		/// <summary>
		/// Constructor! you know! constructs a class.
		/// </summary>
		public TicketHub(
			IRedisClientsManager redisClientsManager,
			IAdvocateService advocateService,
			ICachedBrandRepository cachedBrandRepository,
			ITimestampService timestampService,
			IBrandService brandService)
		{
			_redisClientsManager = redisClientsManager;
			_advocateService = advocateService;
			_cachedBrandRepository = cachedBrandRepository;
			_timestampService = timestampService;
			_brandService = brandService;
		}

		/// <summary>
		/// In case of a client, it will add him to the group to be notified of an advocate joining.
		/// In case of an advocate, it will add him to the list of online advocates and notifies the client.
		/// </summary>
		public override async Task OnConnectedAsync()
		{
			if (Context.User.IsInRole(SolvRoles.Client))
			{
				var brandId = await _brandService.GetClientBrandId(Context.User.GetId());
				await AddClientToBrandGroup(brandId);
				await SendOnlineAdvocateList(brandId);
			}
			else if (Context.User.IsInRole(SolvRoles.Advocate) || Context.User.IsInRole(SolvRoles.SuperSolver))
			{
				// Add the advocate connection to his group for further notifications.
				await SetAdvocateOnlineStatus(true);
			}
		}

		/// <summary>
		/// In case of a client, it will remove him from the group to be notified of an advocate joining.
		/// In case of an advocate, it will add him to the list of online advocates and notifies the client.
		/// </summary>
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			// Get the brand id associated with the client (currently it should only be one)
			if (Context.User.IsInRole(SolvRoles.Client))
			{
				var brandId = await _brandService.GetClientBrandId(Context.User.GetId());
				await RemoveClientFromBrandGroup(brandId);
			}
			else if (Context.User.IsInRole(SolvRoles.Advocate) || Context.User.IsInRole(SolvRoles.SuperSolver))
			{
				// Add the advocate connection from his group.
				await SetAdvocateOnlineStatus(false);
			}
		}

		private async Task SetAdvocateOnlineStatus(bool isOnline)
		{
			var advocateId = Context.User.GetId();
			var brandIds = await _cachedBrandRepository.GetActiveBrandsIds(advocateId);
			var connectionId = Context.ConnectionId;

			using (var client = _redisClientsManager.GetClient())
			{
				var connections = client.AdvocateConnections(_timestampService, advocateId);
				if (isOnline) connections.Add(connectionId); else connections.Remove(connectionId);

				var anyConnectionAlive = connections.Count > 0;

				foreach (var brandId in brandIds)
				{
					if (isOnline)
					{
						client.BrandOnlineAdvocates(_timestampService, brandId).Add(advocateId);
						await AddClientToBrandGroup(brandId);
						await Clients.Group(GetBrandGroupName(brandId)).SendAsync(ADVOCATE_ONLINE, advocateId);
					}
					else
					{
						await RemoveClientFromBrandGroup(brandId);
						if (anyConnectionAlive == false)
						{
							// No connection for this advocate, he is offline, update all his brands.
							client.BrandOnlineAdvocates(_timestampService, brandId).Remove(advocateId);
							await Clients.Group(GetBrandGroupName(brandId)).SendAsync(ADVOCATE_OFFLINE, advocateId);
						}
					}
				}
			}
		}

		/// <summary>
		/// Sends the list of online advocates to the brand passed.
		/// This is called when a client connects the first time.
		private async Task SendOnlineAdvocateList(Guid brandId)
		{
			using (var client = _redisClientsManager.GetClient())
			{
				var list = client.BrandOnlineAdvocates(_timestampService, brandId).GetAll();
				await Clients.Caller.SendAsync(ADVOCATE_PUSH_ONLINE_LIST, list.ToArray());
			}
		}

		/// <summary>
		/// Add the client connection to a group (identified by the brand Id) for future notification.
		/// </summary>
		private Task AddClientToBrandGroup(Guid brandId) => Groups.AddToGroupAsync(Context.ConnectionId, GetBrandGroupName(brandId));

		/// <summary>
		/// Remove the client connection from the brand notification group because it is disconnected.
		/// </summary>
		private Task RemoveClientFromBrandGroup(Guid brandId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, GetBrandGroupName(brandId));

		/// <summary>
		/// TODO: It seems that this method no longer needed (judging from the goup name who is not used by anyone)
		/// TODO: To be removed when frontend no longer invokes this one
		/// </summary>
		public Task Subscribe(List<string> ticketIds) => Task.CompletedTask;

		/// <summary>
		/// TODO: It seems that this method no longer needed (judging from the goup name who is not used by anyone)
		/// TODO: To be removed when frontend no longer invokes this one
		/// </summary>
		public Task Unsubscribe(List<string> ticketIds) => Task.CompletedTask;
	}
}