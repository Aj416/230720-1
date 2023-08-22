using System;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.DataAnnotations;

namespace Tigerspike.Solv.Services.Fraud.Domain
{
	public class RuleRiskLevelGlobalIndex : IGlobalIndex<Rule>
	{

		/// <summary>
		/// Gets or sets the Risk of the rule.
		/// </summary>
		[HashKey]
		public string Risk { get; set; }

		/// <summary>
		/// Gets or sets the statuses of the ticket to which the rule applies.
		/// </summary>
		[Index]
		public int TicketStatus { get; set; }

		/// <summary>
		/// Gets or sets the rule id.
		/// </summary>
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
	}
}