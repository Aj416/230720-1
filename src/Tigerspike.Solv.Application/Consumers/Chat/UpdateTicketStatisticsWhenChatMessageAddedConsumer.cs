using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class UpdateTicketStatisticsWhenChatMessageAddedConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly IMediatorHandler _mediator;

		public UpdateTicketStatisticsWhenChatMessageAddedConsumer(IMediatorHandler mediator)
		{
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			if (new[] {UserType.Advocate, UserType.SuperSolver, UserType.Customer}.Select(x => (int)x)
				.Contains(notification.SenderType))
			{
				await _mediator.SendCommand(new UpdateTicketMessageStatisticsCommand(notification.ConversationId,
					notification.CreatedDate, notification.SenderType));
			}
		}
	}
}