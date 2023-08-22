using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketTagsCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The tags to be set on ticket
		/// </summary>
		public Guid[] TagIds { get; }

		/// <summary>
		/// Gets or sets level for a ticket.
		/// </summary>
		public TicketLevel? Level { get; set; }

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="ticketId"></param>
		public SetTicketTagsCommand(Guid ticketId, Guid[] tagIds, TicketLevel? level) =>
			(TicketId, TagIds, Level) = (ticketId, tagIds, level);

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() => TicketId != Guid.Empty;
	}
}
