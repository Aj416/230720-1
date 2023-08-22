using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SendFirstAdvocateResponseInChatEmailCommand : Command<bool>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The authorization token for customer
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// The ticket question
		/// </summary>
		public string Question { get; }

		/// <summary>
		/// Recipient email
		/// </summary>
		public string Email { get; }

		/// <summary>
		/// The brand id of the ticket being solved
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// The id of the current solver
		/// </summary>
		public Guid AdvocateId { get; }

		/// <summary>
		/// The first name of the customer
		/// </summary>
		public string CustomerFirstName { get; set; }

		/// <summary>
		/// The constructor (obviously).
		/// </summary>
		public SendFirstAdvocateResponseInChatEmailCommand(Guid ticketId, Guid brandId, Guid advocateId, string token, string question, string email, string customerFirstName)
		{
			TicketId = ticketId;
			Token = token;
			Question = question;
			Email = email;
			BrandId = brandId;
			AdvocateId = advocateId;
			CustomerFirstName = customerFirstName;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		Token != null &&
		Question != null &&
		!string.IsNullOrWhiteSpace(Email) &&
		BrandId != Guid.Empty &&
		AdvocateId != Guid.Empty &&
		!string.IsNullOrWhiteSpace(CustomerFirstName);

	}
}