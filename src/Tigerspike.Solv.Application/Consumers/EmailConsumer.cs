using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Consumers
{
	public class EmailConsumer : IConsumer<ReceiveEmailMessageCommand>
	{
		private readonly ILogger<EmailConsumer> _logger;
		private readonly IChatService _chatService;
		private readonly IUserRepository _userRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly IMediatorHandler _mediator;

		public EmailConsumer(
			IChatService chatService,
			IUserRepository userRepository,
			ITicketRepository ticketRepository,
			ILogger<EmailConsumer> logger,
			IMediatorHandler mediator
		)
		{
			_chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task Consume(ConsumeContext<ReceiveEmailMessageCommand> context)
		{

			_logger.LogInformation($"Received an email message: {JsonConvert.SerializeObject(context.Message)} ");

			var userId =
				await _userRepository.GetSingleOrDefaultAsync(predicate: u => u.Email == context.Message.CustomerEmail,
					selector: u =>(Guid?) u.Id);

			if (userId == null)
			{
				_logger.LogError($"User with email {context.Message.CustomerEmail} not found.");
				return;
			}

			// Check that the ticket can receive messages from this customer

			var (customerId, ticketId, ticketTransportType, ticketStatus) =
				await _ticketRepository.GetFirstOrDefaultAsync(
					selector: x => Tuple.Create<Guid?, Guid?, TicketTransportType?, TicketStatusEnum?>(x.Customer.Id, x.Id,
								x.TransportType, x.Status)
							.ToValueTuple(),
					predicate: x => x.Number == context.Message.TicketNumber
				);

			if (ticketId == null)
			{
				_logger.LogError($"Ticket not found for ticket with number #{context.Message.TicketNumber}");
				return;
			}

			if (customerId == null)
			{
				_logger.LogError($"Customer not found for ticket with number #{context.Message.TicketNumber}");
				return;
			}

			if (userId != customerId)
			{
				_logger.LogError($"User {customerId} is not the customer for the ticket {ticketId}");
				return;
			}

			if (ticketStatus != TicketStatusEnum.Assigned && ticketStatus != TicketStatusEnum.Solved)
			{
				_logger.LogError($"Ticket {ticketId}  of status {ticketStatus} cannot receive emails from customer {customerId}");
				return;
			}

			if (ticketTransportType != TicketTransportType.Email)
			{
				_logger.LogError($"Ticket {ticketId} cannot receive emails as its transport type is not email.");
				return;
			}

			// Reopen ticket if it's solved and the customer sent a message
			if (ticketStatus == TicketStatusEnum.Solved)
			{
				await _mediator.SendCommand(new ReopenTicketCommand(ticketId.Value));
			}

			// Create the message model and add it

			var request = new MessageAddModel
			{
				AuthorId = userId.Value,
				Message = context.Message.Message,
				ClientMessageId = Guid.NewGuid(),
				SenderType = (int)UserType.Customer,
			};

			await _chatService.AddMessage(ticketId.Value, request);
		}
	}
}