using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models.Fraud
{
	/// <summary>
	/// Fraud ticket details.
	/// </summary>
	public class FraudTicketModel
	{
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
		/// Rules applied to ticket in fraud analysis that are valid.
		/// </summary>
		public IEnumerable<RuleModel> Rules { get; set; }

		/// <summary>
		/// Fraud Risk level as per rules.
		/// </summary>
		public RiskLevel FraudRiskLevel => (RiskLevel)Rules.Select(x => (int?)x.Risk).OrderByDescending(x => x).First();
	}

	/// <summary>
	/// For fetching Rule.
	/// </summary>
	public class RuleModel
	{
		/// <summary>
		/// Gets or sets the description of the rule.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Risk of the rule.
		/// </summary>
		public RiskLevel Risk { get; set; }
	}
}