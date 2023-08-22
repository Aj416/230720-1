using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Aws.DynamoDb;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Infrastructure.Repositories
{
	/// <inheritdoc /> 
	public class RuleRepository : IRuleRepository
	{
		private readonly IPocoDynamo _db;

		/// <summary>
		/// RuleRepository Parameterised constructor.
		/// </summary>
		public RuleRepository(IPocoDynamo db) => _db = db;

		/// <inheritdoc /> 
		public Rule AddOrUpdateRule(Rule rule) => _db.PutItem(rule);

		/// <inheritdoc /> 
		public void DeleteRule(Guid ruleId) =>
			_db.DeleteItem<Rule>(new DynamoId { Hash = ruleId.ToString() });

		/// <inheritdoc /> 
		public List<string> GetByRiskLevel(RiskLevel riskLevel)
		{
			var result = _db.FromQueryIndex<RuleRiskLevelGlobalIndex>(x =>
				x.Risk.Equals(riskLevel.ToString(), StringComparison.InvariantCultureIgnoreCase))
				.Exec();

			return result.Select(x => x.Id).ToList();
		}

		/// <inheritdoc /> 
		public List<Rule> GetRules(int ticketStatus)
		{
			return _db
				.FromQuery<Rule>()
				.KeyCondition(r => r.TicketStatus == ticketStatus)
				.Filter( r => r.Enabled)
				.Exec().ToList();
		}
	}
}