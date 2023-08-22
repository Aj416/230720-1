using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketCategoryCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The category identifier.
		/// </summary>
		public Guid CategoryId { get; }

		/// <summary>
		/// the advocate identifier.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <param name="categoryId">The category identifier.</param>
		/// <param name="advocateId">The advocate identifier.</param>
		public SetTicketCategoryCommand(Guid ticketId, Guid categoryId, Guid advocateId)
		{
			TicketId = ticketId;
			CategoryId = categoryId;
			AdvocateId = advocateId;
		}

		public override bool IsValid() => TicketId != Guid.Empty && CategoryId != Guid.Empty && AdvocateId != Guid.Empty;
	}
}
