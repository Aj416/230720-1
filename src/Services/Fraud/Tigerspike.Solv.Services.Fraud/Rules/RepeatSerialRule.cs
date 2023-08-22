using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	/// <summary>
	/// Rule to validate if a serial number repeats more than once in three days.
	/// </summary>
	public class RepeatSerialRule : IRule
	{
		/// <inheritdoc />
		public Guid RuleId { get; set; }

		/// <inheritdoc />
		public IDetectFraudCommand Message { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="message">Fraud data points.</param>
		/// <param name="ruleId">Rule identifier.</param>
		public RepeatSerialRule(IDetectFraudCommand message, Guid ruleId)
		{
			Message = message;
			RuleId = ruleId;
		}

		/// <inheritdoc />
		public Task<bool> IsMatchAsync() => Task.FromResult(Message.SerialNumberInfoForLastThreeDays.Count > 1);

		/// <inheritdoc />
		public bool IsValid() => Message.SerialNumberInfoForLastThreeDays.Count > default(int);
	}
}
