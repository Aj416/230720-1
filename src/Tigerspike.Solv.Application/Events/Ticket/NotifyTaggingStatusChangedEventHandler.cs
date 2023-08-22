using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.Events.Ticket
{
	/// <summary>
	/// An event handler class for handling the tagging complete events
	/// </summary>
    public class NotifyTaggingStatusChangedEventHandler :
        INotificationHandler<TicketTagsStatusChangedEvent>
    {
		#region Private Methods

		/// <summary>
		/// The ticket hub
		/// </summary>
		private readonly IHubContext<TicketHub> _ticketHub;
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor to initialize properties
		/// </summary>
		/// <param name="ticketHub"></param>		
		public NotifyTaggingStatusChangedEventHandler(IHubContext<TicketHub> ticketHub) => _ticketHub = ticketHub;
		#endregion

		#region Public Methods

		/// <summary>
		/// The handler for TicketTagsStatusChangedEvent
		/// </summary>
		/// <param name="notification"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Handle(TicketTagsStatusChangedEvent notification, CancellationToken cancellationToken) => await SendTaggingCompleteNotification(notification.TicketId, notification.IsTaggingCompleted, notification.AdvocateId, notification.AdvocateFirstName, notification.AdvocateCsat);
		#endregion

		#region Private Methods

		/// <summary>
		/// A helper method to send notification to the hub 
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="taggingStatus"></param>
		/// <param name="advocateId"></param>
		/// <param name="advocateFirstName"></param>
		/// <param name="advocateCsat"></param>
		/// <returns></returns>
		private Task SendTaggingCompleteNotification(Guid ticketId, bool taggingStatus, Guid? advocateId, string advocateFirstName, decimal? advocateCsat)
		{
			var userIds = new Guid?[] { advocateId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			return _ticketHub.Clients.Users(userIds)
			.SendAsync(TicketHubConstants.TAGGING_STATUS_CHANGED, new TicketTaggingStatusChangedModel(ticketId, taggingStatus, advocateId, advocateFirstName, advocateCsat));
		}
		#endregion
	}
}