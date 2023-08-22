using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketClosedEvent : Event
	{
		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The brand identifier
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The ticket reference id
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// The flag to indicate if it is a practice ticket
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// The source
		/// </summary>
		public string SourceName { get; set; }

		/// <summary>
		/// The advocate id at the time of closure
		/// (could have been changed if a Super solver got in the Middle)
		/// </summary>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The customer associated with the ticket
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The assigned advocate's first name
		/// </summary>
		public string AdvocateFirstName { get; set; }

		/// <summary>
		/// The assigned advocate's CSAT score
		/// </summary>
		public decimal? AdvocateCsat { get; set; }

		/// <summary>
		/// The enum to indicate the ticket is closed by
		/// </summary>
		public ClosedBy ClosedBy { get; set; }

		/// <summary>
		/// The ticket transport type enum
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// The thread identifier
		/// </summary>
		public string ThreadId { get; set; }

		/// <summary>
		/// The ticket tag status
		/// </summary>
		public TicketTagStatus? TagStatus { get; set; }
	}
}