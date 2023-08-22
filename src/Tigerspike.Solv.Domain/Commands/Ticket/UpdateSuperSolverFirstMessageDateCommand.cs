using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class UpdateSuperSolverFirstMessageDateCommand : Command<Unit>
	{
		public Guid TicketId { get; }

		public DateTime CreatedDate { get; }

		public string SenderType { get; }

		public UpdateSuperSolverFirstMessageDateCommand(Guid ticketId, DateTime createdDate, string senderType)
		{
			TicketId = ticketId;
			CreatedDate = createdDate;
			SenderType = senderType;
		}


		public override bool IsValid() =>
			TicketId != Guid.Empty &&
			CreatedDate != DateTime.MinValue &&
			SenderType.IsNotEmpty();
	}
}
