using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class TrackingDetail : ICreatedDate
	{
		/// <summary>
		/// Gets or sets the Id of IP address.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the Id of User.
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the User.
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Gets or sets the Id of Ticket.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Gets or sets the IP Address of User.
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the browser used by User.
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the Event when IP is stored.
		/// </summary>
		public string Event { get; set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		public TrackingDetail() { }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		public TrackingDetail(Guid userId, Guid ticketId, string ipAddress, string userAgent, string eventName)
		{
			UserId = userId;
			TicketId = ticketId;
			IpAddress = ipAddress;
			UserAgent = userAgent;
			Event = eventName;
		}
	}
}
