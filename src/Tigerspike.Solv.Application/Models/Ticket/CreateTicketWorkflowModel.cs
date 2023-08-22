using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Ticket
{
	/// <summary>
	/// The ticket creation workflow DTO model
	/// </summary>
	public class CreateTicketWorkflowModel
	{
		/// <summary>
		/// The ticket identifier
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// The reference identifier
		/// </summary>
		public string ReferenceId { get; set; }

		/// <summary>
		/// The brand of the ticket
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// The customer associated with the ticket
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// The question
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// The source identifier
		/// </summary>
		public int? SourceId { get; set; }

		/// <summary>
		/// The source name
		/// </summary>
		public string SourceName { get; set; }

		/// <summary>
		/// The transport type of the ticket
		/// </summary>
		public TicketTransportType TransportType { get; set; }

		/// <summary>
		/// The flag to indicate if ticket is a practice ticket or not
		/// </summary>
		public bool IsPractice { get; set; }

		/// <summary>
		/// The thread id
		/// </summary>
		public string ThreadId { get; set; }

		/// <summary>
		/// The current ticket level
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// The culture
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		/// The timestamp
		/// </summary>
		public DateTime Timestamp { get; set; }
	}
}
