using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Events.Invoicing;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Chat.SignalR
{
	public class InvoiceEventsNotificationHandler : INotificationHandler<PaymentCreatedEvent>
	{
		private readonly IHubContext<TicketHub> _ticketHub;
		private readonly IRedisClientsManager _redisClientsManager;

		public InvoiceEventsNotificationHandler( IHubContext<TicketHub> ticketHub, IRedisClientsManager redisClientsManager)
		{
			_ticketHub = ticketHub;
			_redisClientsManager = redisClientsManager;
		}

		public async Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AdvocateId.HasValue)
			{
				using var client = _redisClientsManager.GetClient();

				// invalidate statistics changed due to making a payment for an advocate.
				client.Remove(PreviousWeekStatisticsPeriodKey(notification.AdvocateId.Value));

				// Notify the advocate that the statistics has changed.
				await _ticketHub.Clients.User(notification.AdvocateId.ToString()).SendAsync(TicketHubConstants.ADVOCATE_STATISTICS_UPDATED);
			}
		}
	}
}