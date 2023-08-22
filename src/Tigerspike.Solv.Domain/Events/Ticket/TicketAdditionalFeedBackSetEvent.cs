using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketAdditionalFeedBackSetEvent : Event
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid TicketId { get; }

		/// <summary>
		/// The advocate identifier.
		/// </summary>
		public Guid? AdvocateId { get; }

		/// <summary>
		/// The brand identifier.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Determines if ticket is in practice mode.
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// Determines the transport type for specific ticket.
		/// </summary>
		public TicketTransportType TransportType { get; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="advocateId">Advocate identifier.</param>
		/// <param name="brandId">Brand identifier.</param>
		/// <param name="isPractice">If ticket is in practice mode.</param>
		/// <param name="transportType">Transport type for specific ticket.</param>
		public TicketAdditionalFeedBackSetEvent(Guid ticketId, Guid? advocateId, Guid brandId, bool isPractice, TicketTransportType transportType)
		{
			TicketId = ticketId;
			AdvocateId = advocateId;
			BrandId = brandId;
			IsPractice = isPractice;
			TransportType = transportType;
		}

	}
}
