using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace  Tigerspike.Solv.Application.Events.Ticket
{
	/// <summary>
	/// An event handler class for handling the tags disabled events
	/// </summary>
	public class NotifyTicketTagsDisabledEventHandler :
		INotificationHandler<TicketTagsDisabledEvent>
	{
		#region Private Properties

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
		public NotifyTicketTagsDisabledEventHandler(IHubContext<TicketHub> ticketHub) => _ticketHub = ticketHub;
		#endregion

		#region Public Methods

		/// <summary>
		/// The handler for TicketTagsDisabledEvent
		/// </summary>
		/// <param name="notification"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task Handle(TicketTagsDisabledEvent notification, CancellationToken cancellationToken) => SendDisableTagsNotification(notification.TicketId, notification.AdvocateId, notification.DisabledTags);
		#endregion

		#region Private Methods

		/// <summary>
		/// A helper method to send notification to the hub 
		/// </summary>
		/// <param name="ticketId"></param>
		/// <param name="advocateId"></param>
		/// <param name="tagIds"></param>
		/// <returns></returns>
		private Task SendDisableTagsNotification(Guid ticketId, Guid advocateId, List<Guid> tagIds)
		{
			var userIds = new Guid?[] { advocateId }
				.Where(x => x.HasValue)
				.Select(s => s.ToString()).ToList();

			return _ticketHub.Clients.Users(userIds)
			.SendAsync(TicketHubConstants.TICKET_TAGS_DISABLED_ADDED, new TicketTagsDisabledModel(ticketId, tagIds));
		}
		#endregion
	}
}