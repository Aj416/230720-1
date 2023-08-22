using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	/// <summary>
	/// A handler for TicketAdvocateChangedEvent to tell the FE what to do
	/// when the ticket's advocate has been changed.
	/// </summary>
	public class NotifyAdvocateChangedEventHandler :
		INotificationHandler<TicketAdvocateChangedEvent>,
		INotificationHandler<TicketPendingStatusUpdatedEvent>
	{
		private readonly IHubContext<TicketHub> _ticketHub;

		public NotifyAdvocateChangedEventHandler(IHubContext<TicketHub> ticketHub) => _ticketHub = ticketHub;

		public async Task Handle(TicketAdvocateChangedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.OldAdvocateId.HasValue)
			{
				// Tell the old advocate that the ticket is not his anymore.
				await _ticketHub.Clients.User(notification.OldAdvocateId.ToString())
				.SendAsync(TicketHubConstants.TICKET_REMOVED, new { notification.TicketId });
			}

			if (notification.NewAdvocateId.HasValue)
			{
				// Tell the new advocate that a ticket has been added to his pool.
				await _ticketHub.Clients.User(notification.NewAdvocateId.ToString())
				.SendAsync(TicketHubConstants.TICKET_ADDED, new { notification.TicketId });
			}
		}

		public async Task Handle(TicketPendingStatusUpdatedEvent notification, CancellationToken cancellationToken)
		{
			if (notification.AdvocateId.HasValue)
			{
				if (notification.Status == TicketStatusEnum.New)
				{
					// Tell the advocate that a pending ticket has been added to his pool.
					await _ticketHub.Clients.User(notification.AdvocateId.ToString())
					.SendAsync(TicketHubConstants.PENDING_TICKET_ADDED, new { notification.TicketId, notification.BrandId });
				}
				if (notification.Status == TicketStatusEnum.Closed)
				{
					// Tell the advocate that a pending ticket has been removed from his pool.
					await _ticketHub.Clients.User(notification.AdvocateId.ToString())
					.SendAsync(TicketHubConstants.PENDING_TICKET_REMOVED, new { notification.TicketId, notification.BrandId });
				}
			}
		}
	}
}
