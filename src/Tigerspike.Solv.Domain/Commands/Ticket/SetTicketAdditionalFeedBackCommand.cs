using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SetTicketAdditionalFeedBackCommand : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The additional feedback provided by customer for specific ticket.
		/// </summary>
		public string AdditionalFeedBack { get; }

		/// <summary>
		/// Paramterised constructor.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <param name="additionalFeedBack">The additional feedback provided by customer for specific ticket.</param>
		public SetTicketAdditionalFeedBackCommand(Guid ticketId, string additionalFeedBack)
		=> (TicketId, AdditionalFeedBack) = (ticketId, additionalFeedBack);

		public override bool IsValid() => TicketId != Guid.Empty && !string.IsNullOrEmpty(AdditionalFeedBack);
	}
}
