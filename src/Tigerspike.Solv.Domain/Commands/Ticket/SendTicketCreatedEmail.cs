using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// Sends a Ticket received (Query submitted) confirmation email
	/// </summary>
	public class SendTicketCreatedEmail : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The ticket question
		/// </summary>
		public string Question { get; }

		/// <summary>
		/// The Id of the customer
		/// </summary>
		public Guid CustomerId { get; }

		/// <summary>
		/// Culture of the ticket
		/// </summary>
		public string Culture { get;}

		/// <summary>
		/// The Id of the brand
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SendTicketCreatedEmail(Guid ticketId, string question, Guid customerId, Guid brandId, string culture)
		{
			TicketId = ticketId;
			Question = question;
			CustomerId = customerId;
			BrandId = brandId;
			Culture = culture;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		Question != null &&
		BrandId != Guid.Empty &&
		CustomerId != Guid.Empty;

	}
}