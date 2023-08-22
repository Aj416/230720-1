using System;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class ShortInProgressHighRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public ShortInProgressHighRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		public Task<bool> IsMatchAsync() => Task.FromResult(Message.ResponseTimes.Where(rt => rt < 5).Count() > 4);

		public bool IsValid() => (Message.ResponseTimes?.Any() ?? false) && Message.CurrentResponseTimeInMinutes.HasValue && Message.CurrentResponseTimeInMinutes.Value < 5;
	}
}