using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class RepeatSerialDifferentCxRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public RepeatSerialDifferentCxRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		/// <inheritdoc />
		public Task<bool> IsMatchAsync()
		{
			return Task.FromResult(Message.SerialNumberInfoForLastWeek.Count > 1);
		}

		/// <inheritdoc />
		public bool IsValid()
		{
			return Message.SerialNumberInfoForLastWeek.Count > default(int);
		}
	}
}