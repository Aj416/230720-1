using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The complete ticket workflow model
	/// </summary>
	public class CompleteTicketWorkflowModel
	{
		/// <summary>
		/// The ticket id
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The advocate id at the time of closure
		/// (could have been changed if a Super solver got in the Middle)
		/// </summary>
		/// <value></value>
		public Guid? AdvocateId { get; set; }

		/// <summary>
		/// The customer id 
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The advocate first name
		/// </summary>
		public string AdvocateFirstName { get; set; }

		/// <summary>
		/// The advocate CSAT score
		/// </summary>
		public decimal? AdvocateCsat { get; set; }

		/// <summary>
		/// The ticket closed by enum
		/// </summary>
		public ClosedBy ClosedBy { get; set; }

		/// <summary>
		/// The incoming type of the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// The status of the tags
		/// </summary>
		public TicketTagStatus TagStatus { get; set; }
	}
}