using System;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public class RepeatCxIpRule : IRule
	{
		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <inheritdoc />
		public Guid RuleId { get; set; }

		public RepeatCxIpRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		/// <inheritdoc />
		public Task<bool> IsMatchAsync()
		{
			return Task.FromResult(
				Message.CustomerInfoForLastDay.Where(x => x.Key == CustomerDetail.Ip).ToList().Count > 2);
		}

		/// <inheritdoc />
		public bool IsValid()
		{
			return Message.CustomerInfoForLastDay.Count > default(int);
		}
	}
}