using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class IpMatchRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public IpMatchRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		public Task<bool> IsMatchAsync()
		{
			return Task.FromResult(Message.CustomerIp.Equals(Message.SolverIp, StringComparison.InvariantCultureIgnoreCase));
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(Message.CustomerIp) && !string.IsNullOrEmpty(Message.SolverIp);
		}
	}
}