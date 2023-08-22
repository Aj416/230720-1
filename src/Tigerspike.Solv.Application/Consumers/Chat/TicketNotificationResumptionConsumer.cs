using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class TicketNotificationResumptionConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly ITicketAutoResponseService _ticketAutoResponseService;
		private readonly ICachedTicketRepository _cachedTicketRepository;

		public TicketNotificationResumptionConsumer(
			ITicketAutoResponseService ticketAutoResponseService,
			ICachedTicketRepository cachedTicketRepository
		)
		{
			_ticketAutoResponseService = ticketAutoResponseService;
			_cachedTicketRepository = cachedTicketRepository;
		}

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			var solverUserType = new[] { UserType.Advocate, UserType.SuperSolver }.Select(x => (int)x).ToArray();
			if (solverUserType.Contains(notification.SenderType))
			{
				var ticketContext = await _cachedTicketRepository.GetNotificationResumptionFlowContext(notification.ConversationId);
				if (ticketContext?.Canceled == false)
				{
					await _ticketAutoResponseService.CancelResponses(ticketContext.BrandId, ticketContext.TicketId, new[] { BrandAdvocateResponseType.NotificationResumptionTicketAbandoned });
					ticketContext.Canceled = true;
					_cachedTicketRepository.SetNotificationResumptionFlowContext(ticketContext);
				}
			}
		}
	}
}