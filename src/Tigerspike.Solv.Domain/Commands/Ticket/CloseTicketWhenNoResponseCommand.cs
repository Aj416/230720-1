using System;
using Tigerspike.Solv.Core.Bus;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class CloseTicketWhenNoResponseCommand : IScheduledJob
	{
		public string JobId => $"{nameof(CloseTicketWhenNoResponseCommand)}-{TicketId}";

		public Guid TicketId { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected CloseTicketWhenNoResponseCommand() { }

		public CloseTicketWhenNoResponseCommand(Guid ticketId) => TicketId = ticketId;
	}
}