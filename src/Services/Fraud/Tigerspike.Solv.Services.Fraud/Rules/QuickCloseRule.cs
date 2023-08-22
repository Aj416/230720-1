using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class QuickCloseRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public QuickCloseRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		public Task<bool> IsMatchAsync() => Task.FromResult(Message.CloseTime.Value < 5);

		public bool IsValid() => Message.CloseTime.HasValue;
	}
}