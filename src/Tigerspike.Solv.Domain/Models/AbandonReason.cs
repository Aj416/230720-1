using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class AbandonReason
	{
		public const string ForcedEscalationReasonName = "Forced escalation";
		public const string BlockedAdvocateReasonName = "Advocate was blocked";
		public const string AutoAbandonedReasonName = "Auto-abandoned";

		public Guid Id { get; set; }
		public Guid BrandId { get; set; }
		public string Name { get; set; }
		public bool IsForcedEscalation { get; set; }

		/// <summary>
		/// Indicates that this reason is for (blocked advocate)
		/// </summary>
		public bool IsBlockedAdvocate { get; set; }

		/// <summary>
		/// Indicates that this reason is for auto-abandoned (returning customer flow)
		/// </summary>
		public bool IsAutoAbandoned { get; set; }

		public bool IsActive { get; set; }

		/// <summary>
		/// Ticket flow action to take upon selecting this reason
		/// </summary>
		public TicketFlowAction? Action { get; set; }

		public string Description { get; set; }

		public AbandonReason() { }
		public AbandonReason(Guid brandId, string name, bool isActive, bool isForcedEscalation = false, bool isBlockedAdvocate = false, bool isAutoAbandoned = false, TicketFlowAction? action = null) =>
			(BrandId, Name, IsActive, IsForcedEscalation, IsBlockedAdvocate, IsAutoAbandoned, Action) = (brandId, name, isActive, isForcedEscalation, isBlockedAdvocate, isAutoAbandoned, action);

	}
}
