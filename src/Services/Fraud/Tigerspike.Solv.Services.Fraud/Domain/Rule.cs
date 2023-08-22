using System;
using ServiceStack.DataAnnotations;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Domain
{
	[References(typeof(RuleRiskLevelGlobalIndex))]
	public class Rule
	{
		/// <summary>
		/// Gets or sets the statuses of the ticket to which the rule applies.
		/// </summary>
		[HashKey]
		public int TicketStatus { get; set; }

		/// <summary>
		/// Gets or sets the rule id.
		/// </summary>
		[RangeKey]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		public DateTime CreatedDate { get; set; }

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

		/// <summary>
		/// Gets or sets the flag for enabling the rule.
		/// </summary>
		public bool Enabled { get; set; }
	}
}