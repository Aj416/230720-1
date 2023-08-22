using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// Ticket information that the customer can see.
	/// </summary>
	public class CustomerTicketModel
	{
		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The question asked by the customer in this ticket.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The complexity value submitted by advocate who solved it.
		/// </summary>
		public int? Complexity { get; set; }

		/// <summary>
		/// The CSAT submitted by the customer after closing the ticket.
		/// </summary>
		public int? Csat { get; set; }

		/// <summary>
		/// The NPS submitted by the customer after closing the ticket.
		/// </summary>
		public int? Nps { get; set; }

		/// <summary>
		/// The creation date of the ticket.
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// The last modification date of the ticket.
		/// </summary>
		public DateTime? ModifiedDate { get; set; }

		/// <summary>
		/// The date time the ticket was closed.
		/// </summary>
		public DateTime? ClosedDate { get; set; }

		/// <summary>
		/// Timestamp of escalation
		/// </summary>
		public DateTime? EscalatedDate { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// The value of the status as int.
		/// </summary>
		public int StatusId => (int)Status;

		/// <summary>
		/// The information of the customer who created this ticket.
		/// </summary>
		public UserModel Customer { get; set; }

		/// <summary>
		/// The brand that this ticket belongs to.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// If the ticket in Reserved status, this will be the date of expiry.
		/// </summary>
		public DateTime? ReservationExpiryDate { get; set; }

		/// <summary>
		/// The advocate that is currenlty assigned for the ticket.
		/// </summary>
		public CustomerAdvocateModel Advocate { get; set; }

		/// <summary>
		/// The advocates that were ever associated with the ticket.
		/// </summary>
		public IEnumerable<CustomerAdvocateModel> AllAdvocates { get; set; }

		/// <summary>
		/// Transport type used for the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// The title of the advocate that a brand has specified.
		/// </summary>
		public string AdvocateTitle { get; set; }

		/// <summary>
		/// Customer feedback relevant to support.
		/// </summary>
		public string AdditionalFeedBack { get; set; }
	}
}
