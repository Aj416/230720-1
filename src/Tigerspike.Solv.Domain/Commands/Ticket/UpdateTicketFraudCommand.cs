using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Commands.Ticket
{
	public class UpdateTicketFraudCommand : Command<Unit>
	{
		public Guid TicketId { get; }

		public bool IsFraudSuspected { get; }

		public RiskLevel? RiskLevel { get; set; }

		public Dictionary<string, RiskLevel> Risks { get; }

		public UpdateTicketFraudCommand(Guid ticketId, bool isFraudSuspected, RiskLevel? riskLevel,
			Dictionary<string, RiskLevel> risks)
		{
			TicketId = ticketId;
			IsFraudSuspected = isFraudSuspected;
			RiskLevel = riskLevel;
			Risks = risks;
		}

		public UpdateTicketFraudCommand(Guid ticketId, bool isFraudSuspected, int? riskLevel,
			Dictionary<string, int> risks)
		{
			TicketId = ticketId;
			IsFraudSuspected = isFraudSuspected;
			RiskLevel = riskLevel.HasValue ? (RiskLevel?) riskLevel.Value : null;
			Risks = risks?.ToDictionary(x => x.Key, x => (RiskLevel) x.Value);
		}

		public override bool IsValid()
		{
			return TicketId != Guid.Empty && IsFraudSuspected
				? RiskLevel.HasValue && Risks.Count > 0
				: RiskLevel == null && (Risks == null || Risks.Count == 0);
		}
	}
}