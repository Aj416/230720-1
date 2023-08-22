using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	/// <summary>
	/// Sends a Ticket received (Query submitted) confirmation email
	/// </summary>
	public class SendTicketSposEmail : Command<Unit>
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The Id of the customer
		/// </summary>
		public Guid CustomerId { get; }


		/// <summary>
		/// The Id of the brand
		/// </summary>
		public Guid BrandId { get; }

		/// <summary>
		/// Determines whether a spos mail needs to be sent.
		/// </summary>
		public bool SendSposMail { get; set; }

		/// <summary>
		/// The constructor.
		/// </summary>
		public SendTicketSposEmail(Guid ticketId, Guid customerId, Guid brandId, bool sendSposMail)
		{
			TicketId = ticketId;
			CustomerId = customerId;
			BrandId = brandId;
			SendSposMail = sendSposMail;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		/// <returns></returns>
		public override bool IsValid() =>
		TicketId != Guid.Empty &&
		BrandId != Guid.Empty &&
		CustomerId != Guid.Empty;
	}
}