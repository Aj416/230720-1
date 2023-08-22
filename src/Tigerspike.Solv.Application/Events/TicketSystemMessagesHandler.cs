using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tigerspike.Solv.Application.Constants;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Chat.Application.EventHandlers
{
	/// <summary>
	/// Responsible for generating all system messages for a ticket.
	/// </summary>
	public class TicketSystemMessagesHandler :
		INotificationHandler<TicketReopenedEvent>,
		INotificationHandler<TicketSolvedEvent>,
		INotificationHandler<TicketClosedEvent>,
		INotificationHandler<TicketAcceptedEvent>
	{
		private readonly IChatService _chatService;
		private readonly IMediatorHandler _mediator;

		public TicketSystemMessagesHandler(
			IChatService chatService,
			IMediatorHandler mediator)
		{
			_chatService = chatService;
			_mediator = mediator;
		}


		public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
		{
			await _chatService.AddSystemMessage(notification.TicketId, MessageType.StatusChange,
				ChatMessageConstants.TicketAssigned, notification.AdvocateFirstName);

			await _chatService.FinalizeActiveActions(notification.TicketId);

			// notify the system that notification has been delivered
			await _mediator.RaiseEvent(new TicketAcceptedNotifiedEvent(notification.TicketId, notification.BrandId, notification.AdvocateId, notification.AdvocateFirstName, notification.IsSuperSolver, notification.Level, notification.EscalationReason,
				notification.FirstAssignedDate, notification.AssignedDate
			));
		}

		public async Task Handle(TicketSolvedEvent notification, CancellationToken cancellationToken)
		{
			await _chatService.AddSystemMessage(notification.TicketId, MessageType.SystemMessage,
				new[] { UserType.Advocate, UserType.System }, SystemMessage.TicketSolved);

			if (notification.TransportType == TicketTransportType.Chat)
			{
				await _chatService.AddIsTicketSolvedQuestion(notification.TicketId, notification.AdvocateId);
			}
		}

		public async Task Handle(TicketReopenedEvent notification, CancellationToken cancellationToken)
		{
			await _chatService.AddSystemMessage(notification.TicketId, MessageType.SystemMessage,
				new[] { UserType.Advocate, UserType.System }, SystemMessage.TicketReopened);
		}

		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken)
		{
			// disable all unfinalized actions by now
			await _chatService.FinalizeActiveActions(notification.TicketId);

			if(notification.ClosedBy == ClosedBy.EndChat)
			{
				await _chatService.AddSystemMessage(notification.TicketId, MessageType.StatusChange,
				new[] { UserType.Advocate, UserType.System }, ChatMessageConstants.TicketEndChat);
			}
			else
			{
				await _chatService.AddSystemMessage(notification.TicketId, MessageType.StatusChange,
				new[] { UserType.Advocate, UserType.System }, ChatMessageConstants.TickedClosed);
			}

			await _chatService.AddSystemMessage(notification.TicketId, MessageType.SystemMessage,
				new[] { UserType.Advocate, UserType.System }, SystemMessage.TicketClosed);

			await _mediator.RaiseEvent(new TicketRatingEvent(notification.TicketId, notification.BrandId, notification.TransportType, notification.AdvocateId));
		}

	}
}