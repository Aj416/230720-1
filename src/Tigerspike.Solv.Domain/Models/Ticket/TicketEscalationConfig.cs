using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Configuration for escalation flow of the ticket
	/// </summary>
	public class TicketEscalationConfig
	{
		public Guid Id { get; set; }
		public Guid BrandId { get; set; }
		public int? TicketSourceId { get; set; }
		public int? OpenTimeInSeconds { get; set; }
		public int? RejectionCount { get; set; }
		public int? AbandonedCount { get; set; }
		public string CustomerMessage { get; set; }
	}
}
