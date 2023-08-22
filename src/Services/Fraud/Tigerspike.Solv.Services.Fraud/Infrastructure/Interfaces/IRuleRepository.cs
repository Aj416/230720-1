using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces
{
	/// <summary>
	/// Rule repository to connect with dynamo db.
	/// </summary>
	public interface IRuleRepository
	{
		/// <summary>
		/// Gets the rule list based on the specified ticket status.
		/// </summary>
		/// <param name="ticketStatus">The ticket status.</param>
		/// <returns>The list of rules for the ticket status.</returns>
		List<Rule> GetRules(int ticketStatus);

		/// <summary>
		/// Adds or updates the item in the store.
		/// </summary>
		/// <param name="rule">The rule to add.</param>
		/// <returns>The created or updated rule.</returns>
		Rule AddOrUpdateRule(Rule rule);

		/// <summary>
		/// Deletes the item based on the rule id.
		/// </summary>
		/// <param name="ruleId">The rule id.</param>
		/// <returns></returns>
		void DeleteRule(Guid ruleId);

		/// <summary>
		/// Gets list rule id for specific risk level.
		/// </summary>
		/// <param name="riskLevel">The risk Level.</param>
		/// <returns>List rule id for specific risk level.</returns>
		List<string> GetByRiskLevel(RiskLevel riskLevel);
	}
}