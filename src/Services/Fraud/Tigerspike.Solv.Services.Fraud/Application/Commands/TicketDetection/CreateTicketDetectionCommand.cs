using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Services.Fraud.Application.Commands.TicketDetection
{
	public class CreateTicketDetectionCommand : Command<Unit>
	{
		public Guid TicketId { get; set; }
		public IList<Guid> Rules { get; set; } = new List<Guid>();
		public int Status { get; set; }

		public CreateTicketDetectionCommand(List<Guid> rules, Guid ticketId, int status)
		{
			Rules = rules;
			TicketId = ticketId;
			Status = status;
		}

		public override bool IsValid()
		{
			return Rules.Count > default(int) && TicketId != Guid.Empty;
		}
	}
}