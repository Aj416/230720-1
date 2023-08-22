using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Models.Chat;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Commands
{
	public class ReceiveFromMessengerCommandHandler : CommandHandler, IRequestHandler<ReceiveFromMessengerCommand, Unit>
	{
		private const string ConfirmSolvedAnswer = "yes";
		private readonly ITicketRepository _ticketRepository;
		private readonly IUserRepository _userRepository;
		private readonly IChatService _chatService;

		public ReceiveFromMessengerCommandHandler(
			IChatService chatService,
			IUserRepository userRepository,
			ITicketRepository ticketRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_chatService = chatService;
			_userRepository = userRepository;
			_ticketRepository = ticketRepository;
		}

		public async Task<Unit> Handle(ReceiveFromMessengerCommand request, CancellationToken cancellationToken)
		{
			// Find the user

			var customer = await _userRepository.GetByEmail(request.UserEmail);


			// Get the latest ticket by conversation id

			var ticket = customer != null ?
				await _ticketRepository.GetFirstOrDefaultAsync(
					predicate: t =>
						t.BrandId == request.BrandId && t.ThreadId == request.ConversationId && t.Customer.Id == customer.Id &&
						(t.Status == TicketStatusEnum.New || t.Status == TicketStatusEnum.Assigned || t.Status == TicketStatusEnum.Solved),
					orderBy: o => o.OrderByDescending(t => t.CreatedDate)) : null;

			// If no ticket is found create one

			if (ticket == null)
			{
				await _mediator.SendCommand(
					new CreateTicketCommand(
						request.DisplayName,
						string.Empty,
						request.UserEmail,
						request.Content,
						request.BrandId,
						TicketTransportType.Messenger,
						null,
						threadId: request.ConversationId,
						"Messenger",
						metadata: null
					)
				);
			}
			else
			{
				// Reopen ticket if it's solved and the customer sent a message
				if (ticket.Status == TicketStatusEnum.Solved && request.Content.ToLowerInvariant() != ConfirmSolvedAnswer)
				{
					await _mediator.SendCommand(new ReopenTicketCommand(ticket.Id));
				}
				else if (ticket.Status == TicketStatusEnum.Solved && request.Content.ToLowerInvariant() == ConfirmSolvedAnswer)
				{
					await _mediator.SendCommand(new CloseTicketCommand(ticket.Id, ClosedBy.Customer));

					return Unit.Value;
				}

				// Create the message model and add it to the ticket

				var messageRequest = new MessageAddModel
				{
					AuthorId = customer.Id,
					Message = request.Content,
					ClientMessageId = Guid.NewGuid(),
					SenderType = (int)UserType.Customer,
				};

				await _chatService.AddMessage(ticket.Id, messageRequest);
			}

			return Unit.Value;
		}
	}
}