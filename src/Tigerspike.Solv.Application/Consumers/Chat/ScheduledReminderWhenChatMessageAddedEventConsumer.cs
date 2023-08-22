using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Chat;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class ScheduledReminderWhenChatMessageAddedEventConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<ScheduledReminderWhenChatMessageAddedEventConsumer> _logger;
		private readonly IRedisClientsManager _redisClientsManager;
		private readonly ITicketRepository _ticketRepository;

		public ScheduledReminderWhenChatMessageAddedEventConsumer(
			ITicketRepository ticketRepository,
			IMediatorHandler mediator,
			ILogger<ScheduledReminderWhenChatMessageAddedEventConsumer> logger,
			IRedisClientsManager redisClientsManager)
		{
			_ticketRepository = ticketRepository;
			_mediator = mediator;
			_logger = logger;
			_redisClientsManager = redisClientsManager;
		}

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;
			var ticketTransportType = await _ticketRepository.GetSingleOrDefaultAsync(t => t.TransportType,
				predicate: t => t.Id == notification.ConversationId);

			if (((UserType)notification.SenderType).IsIn(UserType.Advocate, UserType.SuperSolver) && IsCustomerOnline(notification.ConversationId.ToString()) == false &&
			    ticketTransportType == TicketTransportType.Chat && notification.MessageType == (int)MessageType.Message)
			{
				_logger.LogInformation("Scheduling a chat reminder message {0} for ticket {1}",
					notification.Message, notification.ConversationId);

				await _mediator.SendCommand(new ScheduleChatReminderCommand(notification.ConversationId));
			}
		}

		private bool IsCustomerOnline(string conversationId)
		{
			var key = GetCustomerConnectionsKey(conversationId);
			using (var client = _redisClientsManager.GetClient())
			{
				// The customer is online if he has active connections.
				return client.Get<long>(key) > 0;
			}
		}
	}
}