using System;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Models
{
	/// <summary>
	/// For fetching Rule.
	/// </summary>
	public class RuleModel
	{
		/// <summary>
		/// Gets or sets the statuses of the ticket to which the rule applies.
		/// </summary>
		public int TicketStatus { get; set; }

		/// <summary>
		/// Gets or sets the rule id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the unique name of the rule i.e. MatchingIP
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the rule.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the label of the rule.
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the Risk of the rule.
		/// </summary>
		public RiskLevel Risk { get; set; }
	}
}