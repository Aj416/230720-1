using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class SendTicketClosedEmailCommand : Command<bool>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The Id of the brand of this ticket.
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// The id of the advocate.
		/// </summary>
		public Guid AdvocateId { get; }

		/// <summary>
		/// The ticket timestamp
		/// </summary>
		public DateTime CreatedDate { get; }

		/// <summary>
		/// The ticket question
		/// </summary>
		public string Question { get; }

		/// <summary>
		/// The ticket conversation transcript
		/// </summary>
		public string Conversation { get; }

		/// <summary>
		/// Recipient email
		/// </summary>
		public string Email { get; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SendTicketClosedEmailCommand(Guid ticketId, Guid brandId, Guid advocateId, DateTime createdDate, string question, string conversation, string email)
		{
			AdvocateId = advocateId;
			BrandId = brandId;
			TicketId = ticketId;
			CreatedDate = createdDate;
			Question = question;
			Conversation = conversation;
			Email = email;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		BrandId != Guid.Empty &&
		AdvocateId != Guid.Empty &&
		CreatedDate != DateTime.MinValue &&
		Question != null &&
		Conversation != null &&
		!string.IsNullOrWhiteSpace(Email);

	}
}