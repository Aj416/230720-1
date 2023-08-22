using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class ReceiveEmailMessageCommand : Command<Unit>
	{
		public long TicketNumber { get; set; }

		public string CustomerEmail { get; set; }

		public string Subject { get; set; }

		public string Message { get; set; }

		public string EmailMessageId { get; set; }

		public DateTime DateCreated { get; set; }

		public override bool IsValid()
        {
        	return TicketNumber > 0;
        }
	}
}