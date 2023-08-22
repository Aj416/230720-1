using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The ticket creation notification model
	/// </summary>
	public class CreateTicketNotificationModel
	{
		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The brand identifier
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The ticket level
		/// </summary>
		public TicketLevel Level { get; set; }
	}
}
