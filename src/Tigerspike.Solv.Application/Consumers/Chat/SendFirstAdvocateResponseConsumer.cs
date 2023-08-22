using System;
using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Chat;

namespace Tigerspike.Solv.Application.Consumers.Chat
{
	public class SendFirstAdvocateResponseConsumer : IConsumer<IChatMessageAddedEvent>
	{
		private readonly IMediatorHandler _mediator;
		private readonly ITicketRepository _ticketRepository;
		private readonly IJwtService _jwtService;

		public SendFirstAdvocateResponseConsumer(
			IMediatorHandler mediator,
			IJwtService jwtService,
			ITicketRepository ticketRepository)
		{
			_jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
			_ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task Consume(ConsumeContext<IChatMessageAddedEvent> context)
		{
			var notification = context.Message;

			if (notification.SenderType == (int)UserType.Advocate || notification.SenderType == (int)UserType.SuperSolver)
			{
				var firstMessageDate = await _ticketRepository.GetSingleOrDefaultAsync(x => x.FirstMessageDate, x => x.Id == notification.ConversationId);
				if (firstMessageDate == null)
				{
					var ticket = await _ticketRepository.GetFullTicket(x => x.Id == notification.ConversationId);

					if (ticket.TransportType == TicketTransportType.Chat && ticket.Status == TicketStatusEnum.Assigned)
					{
						var customerAuthToken = _jwtService.CreateTokenForTicket(ticket.Id, ticket.Customer.Id);
						await _mediator.SendCommand(new SendFirstAdvocateResponseInChatEmailCommand(ticket.Id, ticket.BrandId, ticket.AdvocateId.Value, customerAuthToken.Token, ticket.Question, ticket.Customer.Email, ticket.Customer.FirstName));
					}
				}
			}
		}
	}
}