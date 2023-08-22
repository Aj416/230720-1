using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Rules
{
	public interface IRule
	{
		/// <summary>
		/// The rule id.
		/// </summary>
		public Guid RuleId { get; set; }

		/// <summary>
		/// The original fraud detection message.
		/// </summary>
		public IDetectFraudCommand Message { get; set; }

		/// <summary>
		/// Checks if the rule has a match.
		/// </summary>
		/// <returns></returns>
		public Task<bool> IsMatchAsync();

		/// <summary>
		/// Determines if the rule is valid.
		/// </summary>
		/// <returns></returns>
		public bool IsValid();
	}
}