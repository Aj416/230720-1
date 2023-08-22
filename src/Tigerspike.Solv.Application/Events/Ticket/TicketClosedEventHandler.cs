using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;

namespace Tigerspike.Solv.Application.Events.Ticket
{
	public class TicketClosedEventHandler : INotificationHandler<TicketClosedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly ICachedTicketRepository _cachedTicketRepository;
		private readonly IChatService _chatService;

		public TicketClosedEventHandler(
			IChatService chatService,
			IMediatorHandler mediator,
			ITicketRepository ticketRepository,
			ICachedTicketRepository cachedTicketRepository)
		{
			_chatService = chatService;
			_mediator = mediator;
			_ticketRepository = ticketRepository;
			_cachedTicketRepository = cachedTicketRepository;
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			if (await _cachedTicketRepository.GetTransportType(notification.TicketId) == TicketTransportType.Chat && notification.AdvocateId != null)
			{
				var ticket = await _ticketRepository.GetSingleOrDefaultAsync(x => new { x.BrandId, x.AdvocateId, x.CreatedDate, x.Question, x.Customer.Email, }, x => x.Id == notification.TicketId);
				var conversation = await _chatService.GetTranscript(notification.TicketId);
				var sendTicketClosedEmailCommand = new SendTicketClosedEmailCommand(notification.TicketId, ticket.BrandId, ticket.AdvocateId.Value, ticket.CreatedDate, ticket.Question, conversation, ticket.Email);

				await _mediator.SendCommand(sendTicketClosedEmailCommand);
			}
		}
	}
}