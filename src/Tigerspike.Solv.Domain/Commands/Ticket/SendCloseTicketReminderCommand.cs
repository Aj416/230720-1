using System;
using MediatR;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SendCloseTicketReminderCommand : Command<Unit>, IScheduledJob
	{
		public Guid NotificationId { get; set; }
		public Guid TicketId { get; set; }
		public Guid AdvocateId { get; set; }
		public string Subject { get; set; }
		public string Header { get; set; }
		public string Body { get; set; }

		public string JobId => $"{nameof(SendCloseTicketReminderCommand)}-{TicketId}-{NotificationId}";

		public SendCloseTicketReminderCommand() { }

		public SendCloseTicketReminderCommand(Guid ticketId, Guid notificationId) => (TicketId, NotificationId) = (ticketId, notificationId);

		public SendCloseTicketReminderCommand(Guid ticketId, Guid notificationId, Guid advocateId, string subject, string header, string body)
		{
			TicketId = ticketId;
			NotificationId = notificationId;
			AdvocateId = advocateId;
			Subject = subject;
			Header = header;
			Body = body;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
			AdvocateId != Guid.Empty &&
			TicketId != Guid.Empty;
	}
}