using System;
using System.Collections.Generic;
using MassTransit.Initializers.PropertyConverters;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Services.Fraud.Application.IntegrationEvents
{
	public class FraudDetectionCompletedEvent : IFraudDetectionCompletedEvent
	{
		/// <inheritdoc />
		public DateTime Timestamp { get; set; }

		/// <inheritdoc />
		public Guid TicketId { get; set; }

		/// <inheritdoc />
		public bool IsFraudSuspected { get; set; }

		/// <inheritdoc />
		public int? RiskLevel { get; set; }

		/// <inheritdoc />
		public Dictionary<string, int> Risks { get; set; }

		public FraudDetectionCompletedEvent(Guid ticketId, bool isFraudSuspected, int? riskLevel,
			Dictionary<string, int> risks)
		{
			Timestamp = DateTime.UtcNow;
			TicketId = ticketId;
			IsFraudSuspected = isFraudSuspected;
			RiskLevel = riskLevel;
			Risks = risks;
		}
	}
}