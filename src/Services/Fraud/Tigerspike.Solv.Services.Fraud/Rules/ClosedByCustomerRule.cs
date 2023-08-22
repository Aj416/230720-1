using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class ClosedByCustomerRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public ClosedByCustomerRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		public Task<bool> IsMatchAsync()
		{
			return Task.FromResult(Message.ClosedByCustomer.Value);
		}

		public bool IsValid()
		{
			return Message.ClosedByCustomer.HasValue;
		}
	}
}