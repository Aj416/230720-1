using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketSposCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; protected set; }

		/// <summary>
		/// SPOS lead indicator
		/// </summary>
		public bool SposLead { get; set; }

		/// <summary>
		/// SPOS lead details
		/// </summary>
		public string SposDetails { get; set; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SetTicketSposCommand(Guid ticketId, bool sposLead, string sposDetails)
		{
			TicketId = ticketId;
			SposLead = sposLead;
			SposDetails = sposDetails;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}