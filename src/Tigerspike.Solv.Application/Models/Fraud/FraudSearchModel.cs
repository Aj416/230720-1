using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Fraud
{
	/// <summary>
	/// FraudSearchModel class.
	/// </summary>
	public class FraudSearchModel
	{
		/// <summary>
		/// The date the application was made in string format.
		/// Its for search functionality.
		/// </summary>
		public string CreatedDateText { get; set; }

		/// <summary>
		/// The brand identifier.
		/// </summary>
		public string BrandId { get; set; }

		/// <summary>
		/// The name of brand.
		/// </summary>
		public string BrandName { get; set; }

		/// <summary>
		/// The fraud status.
		/// </summary>
		public FraudStatus FraudStatus { get; set; }

		/// <summary>
		/// The ticket identifier.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The status of the ticket.
		/// </summary>
		public TicketStatusEnum Status { get; set; }

		/// <summary>
		/// Level of the ticket
		/// </summary>
		public FraudLevel Level { get; set; }

		/// <summary>
		/// The current advocates name.
		/// </summary>
		public string AdvocateName { get; set; }

		/// <summary>
		/// The list of observed risks for a particular ticket.
		/// </summary>
		public IEnumerable<string> FraudRisks { get; set; }

		/// <summary>
		/// The cumulative point for all observed risk.
		/// </summary>
		public string FraudRiskLevel { get; set; }

		/// <summary>
		/// Metadata key value pair.
		/// </summary>
		public IDictionary<string, string> Metadata { get; set; }

		/// <summary>
		/// Gets or sets the IP address of the customer.
		/// </summary>
		public string IpAddress { get; set; }
	}
}