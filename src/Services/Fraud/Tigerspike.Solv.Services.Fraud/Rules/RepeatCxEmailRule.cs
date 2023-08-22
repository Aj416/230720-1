using System;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class RepeatCxEmailRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public RepeatCxEmailRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		public Task<bool> IsMatchAsync()
		{
			return Task.FromResult(
				Message.CustomerInfoForLastDay.Where(x => x.Key == CustomerDetail.Email).ToList().Count > 2);
		}

		public bool IsValid()
		{
			return Message.CustomerInfoForLastDay.Count > default(int);
		}
	}
}