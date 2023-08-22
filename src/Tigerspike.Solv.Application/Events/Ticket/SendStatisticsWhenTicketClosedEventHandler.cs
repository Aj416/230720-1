using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tigerspike.Solv.Application.SignalR;
using Tigerspike.Solv.Domain.Events.Ticket;

namespace Tigerspike.Solv.Application.EventHandlers.Ticket
{
	/// <summary>
	/// Responsible for sending notification about any change in the tickets to the hub
	/// </summary>
	public class SendStatisticsWhenTicketClosedEventHandler :
		INotificationHandler<TicketClosedEvent>
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
		public SendStatisticsWhenTicketClosedEventHandler(IHubContext<TicketHub> ticketHub) => _ticketHub = ticketHub;
		#endregion

		#region Public Methods

		/// <summary>
		/// The handler for TicketClosedEvent
		/// </summary>
		/// <param name="notification"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Handle(TicketClosedEvent notification, CancellationToken cancellationToken) => await SendNotification(notification.AdvocateId.ToString());
		#endregion

		#region Private Methods

		/// <summary>
		/// A helper method to notify the advocate connections group that the statistics has changed
		/// </summary>
		/// <param name="advocateId"></param>
		/// <returns></returns>
		private Task SendNotification(string advocateId) =>  _ticketHub.Clients.User(advocateId).SendAsync(TicketHubConstants.ADVOCATE_STATISTICS_UPDATED);
		#endregion
	}
}
