using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Chat.Application.Commands;
using Tigerspike.Solv.Chat.Infrastructure.Repositories;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Messaging.Ticket;
using Tigerspike.Solv.Services.Chat.Enums;

namespace Tigerspike.Solv.Services.Chat.Application.Consumers
{
	public class TicketAdvocateChangedEventConsumer : IConsumer<ITicketAdvocateChangedEvent>
	{
		private readonly ICachedConversationRepository _cachedConversationRepository;
		private readonly IMediatorHandler _mediator;

		public TicketAdvocateChangedEventConsumer(ICachedConversationRepository cachedConversationRepository,
			IMediatorHandler mediator)
		{
			_cachedConversationRepository = cachedConversationRepository;
			_mediator = mediator;
		}

		public async Task Consume(ConsumeContext<ITicketAdvocateChangedEvent> context)
		{
			var notification = context.Message;

			await _mediator.SendCommand(new UpdateAdvocateForConversation(notification.TicketId,
				(TicketStatusEnum) notification.TicketStatus,
				notification.NewAdvocateId, notification.NewAdvocateFirstName, notification.NewAdvocateCsat));

			_cachedConversationRepository.InvalidateConversation(notification.TicketId);
		}
	}
}