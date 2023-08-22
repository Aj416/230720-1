using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using static Tigerspike.Solv.Core.Constants.CacheKeys;
using static Tigerspike.Solv.Application.SignalR.ChatHubConstants;

namespace Tigerspike.Solv.Application.SignalR
{

	[Authorize(Roles = SolvRoles.Advocate + "," + SolvRoles.SuperSolver + "," + SolvRoles.Customer)]
	public class ChatHub : Hub
	{
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ISchedulerService _schedulerService;
		private readonly ILogger<ChatHub> _logger;
		private readonly IChatService _chatService;

		public ChatHub(
			IRedisClientsManager redisClientsManager,
			IChatService chatService,
			ISchedulerService schedulerService,
			ILogger<ChatHub> logger)
		{
			_redisClientsManager = redisClientsManager;
			_chatService = chatService;
			_schedulerService = schedulerService;
			_logger = logger;
		}

		public override async Task OnConnectedAsync()
		{
			if (Context.User.IsInRole(SolvRoles.Customer))
			{
				await SetCustomerOnlineStatus(true);
			}
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (Context.User != null && Context.User.IsInRole(SolvRoles.Customer))
			{
				await SetCustomerOnlineStatus(false);
			}
		}

		public async Task UserIsTyping(string conversationId) => await SendUserIsTyping(conversationId, true);

		public async Task UserStoppedTyping(string conversationId) => await SendUserIsTyping(conversationId, false);

		private async Task SendUserIsTyping(string conversationId, bool isTyping)
		{
			var user = Context.User;

			if (user == null || string.IsNullOrEmpty(conversationId))
			{
				return;
			}

			var ticketId = Guid.Parse(conversationId);

			var msg = isTyping ? MESSAGE_USERTYPING : MESSAGE_USERSTOPPEDYPING;

			if (user.IsInRole(SolvRoles.Advocate) || user.IsInRole(SolvRoles.SuperSolver))
			{
				var conversation = await _chatService.GetConversation(new Guid(conversationId));

				if (conversation.AdvocateId != null && conversation.AdvocateId == user.GetId().ToString())
				{
					// Notify the customer  that the advocate is typing
					await Clients.User(conversation.CustomerId).SendAsync(msg, conversationId, SolvRoles.Advocate);
				}
				else
				{
					_logger.LogWarning("ChatHub - Solver {0} is not assigned to ticket {1}", user.GetId(),
						ticketId);
				}
			}
			else if (user.IsInRole(SolvRoles.Customer))
			{
				if(user.HasTokenForTicket(ticketId))
				{
					var conversation = await _chatService.GetConversation(new Guid(conversationId));

					// Notify the advocate that the customer is typing
					await Clients.User(conversation.AdvocateId).SendAsync(msg, conversationId, SolvRoles.Customer);
				}
				else
				{
					_logger.LogWarning("ChatHub - Customer {0} does not have token for ticket {1}", user.GetId(),
						ticketId);
				}
			}
			else
			{
				throw new HubException("We haven't implemented anything other than advocate or customer roles");
			}
		}

		/// <summary>
		/// Increase/Decrease the number of connections for a customer for a specific ticket.
		/// Remember that the customer can have two tickets,
		/// but only one of them is open and thus he is online only for one.
		/// </summary>
		/// <param name="isOnline">Is customer online or offline</param>
		private async Task SetCustomerOnlineStatus(bool isOnline)
		{
			var ticketId = Context.User.GetCustomerTicketId();
			var conversation = await _chatService.GetConversation(new Guid(ticketId));
			var key = GetCustomerConnectionsKey(ticketId);
			using var client = _redisClientsManager.GetClient();
			if (isOnline)
			{
				if (client.IncrementValue(key) == 1)
				{
					// If the customer is online for the very first time.
					await Clients.Users(conversation.AdvocateId).SendAsync(CUSTOMER_ONLINE, ticketId);
				}
				// Whenc customer beomces online, no need to send a new-reply reminder email
				await CancelChatReminder(ticketId);
			}
			else
			{
				if(client.Get<long>(key) == 0)
				{
					// In a very rare case, when the customer is online but for some reason the count is still 0!
					// This could be if someone deleted the key manually, or the service stopped unexpectedly.
					await Clients.Users(conversation.AdvocateId).SendAsync(CUSTOMER_OFFLINE, ticketId);
				}
				if (client.DecrementValue(key) == 0)
				{
					// If the customer has closed all windows.
					await Clients.Users(conversation.AdvocateId).SendAsync(CUSTOMER_OFFLINE, ticketId);
				}
			}
		}

		private Task CancelChatReminder(string conversationId)
		{
			var cmd = new SendChatReminderCommand(Guid.Parse(conversationId));
			return _schedulerService.CancelScheduledJob(cmd);
		}
	}

}