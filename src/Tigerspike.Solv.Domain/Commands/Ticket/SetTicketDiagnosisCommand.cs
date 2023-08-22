using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketDiagnosisCommand : Command<bool>
	{
		/// <summary>
		/// Ticket identifier.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Whether escalated ticket was correctly diagnosed or not
		/// </summary>
		public bool? CorrectlyDiagnosed { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="correctlyDiagnosed">Determines if ticket is correctly diagnosed or not.</param>
		public SetTicketDiagnosisCommand(Guid ticketId, bool? correctlyDiagnosed)
		{
			TicketId = ticketId;
			CorrectlyDiagnosed = correctlyDiagnosed;
		}

		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
