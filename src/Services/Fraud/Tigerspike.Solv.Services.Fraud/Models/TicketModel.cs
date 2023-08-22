using System;
using System.Collections.Generic;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Models
{
	/// <summary>
	/// For fetching Ticket.
	/// </summary>
	public class TicketModel
	{
		/// <summary>
		/// Gets or sets the created date
		/// </summary>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the ticket Id.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Gets or sets the customer id.
		/// </summary>
		public Guid CustomerId { get; set; }

		/// <summary>
		/// Gets or sets the current solver assigned to the ticket.
		/// </summary>
		public string AssignedTo { get; set; }

		/// <summary>
		/// Gets or sets the brand Id.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets the brand id of the ticket.
		/// </summary>
		public string BrandName { get; set; }

		/// <summary>
		/// Gets or sets the level of the ticket.
		/// </summary>
		public TicketLevel Level { get; set; }

		/// <summary>
		/// Gets or sets the status of the ticket.
		/// </summary>
		public TicketStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the last fraud result.
		/// </summary>
		public FraudStatus FraudStatus { get; private set; }

		/// <summary>
		/// Rules applied to ticket in fraud analysis that are valid.
		/// </summary>
		public IEnumerable<RuleModel> Rules { get; set; }

		/// <summary>
		/// Customer specific details.
		/// </summary>
		public CustomerModel CustomerDetail { get; set; }

		/// <summary>
		/// Metadata key value pair.
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Gets or sets question asked by customer.
		/// </summary>
		public string Question { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the customer.
		/// </summary>
		public string IpAddress { get; set; }
	}
}