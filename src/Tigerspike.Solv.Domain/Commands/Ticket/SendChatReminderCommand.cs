using System;
using MediatR;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SendChatReminderCommand : Command<Unit>, IScheduledJob
	{
		public Guid TicketId { get; set; }
		public string BrandLogoUrl { get; set; }
		public string BrandName { get; set; }
		public string Question { get; set; }
		public string AdvocateFirstName { get; set; }
		public string ChatUrl { get; set; }
		public string CustomerEmail { get; set; }
		public string Subject { get; set; }
		public string EmailLogoLocation { get; set; }

		public string JobId => $"{nameof(SendChatReminderCommand)}-{TicketId}";

		public SendChatReminderCommand() { }

		public SendChatReminderCommand(Guid ticketId) => TicketId = ticketId;

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
			TicketId != Guid.Empty &&
			BrandName != null &&
			Question != null &&
			AdvocateFirstName != null &&
			ChatUrl != null &&
			CustomerEmail != null;
	}
}