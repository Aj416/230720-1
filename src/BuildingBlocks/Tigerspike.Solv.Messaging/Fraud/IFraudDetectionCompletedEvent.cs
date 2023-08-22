using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Messaging.Fraud
{
	public interface IFraudDetectionCompletedEvent
	{
		/// <summary>
		/// Gets or sets the ticket id.
		/// </summary>
		public Guid TicketId { get; set; }

		/// <summary>
		/// Gets or sets whether the ticket is suspected to be fraudulent.
		/// </summary>
		public bool IsFraudSuspected { get; set; }

		/// <summary>
		/// Gets or sets the risk level if the ticket is fraudulent.
		/// </summary>
		public int? RiskLevel { get; set; }

		/// <summary>
		/// Gets or sets the list of observed risks.
		/// </summary>
		public Dictionary<string, int> Risks { get; set; }

		/// <summary>
		/// Gets or sets the event timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }
	}
}