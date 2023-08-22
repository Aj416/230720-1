using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class TicketStatusChangedModel
	{
		public Guid ConversationId { get; set; }
		public TicketStatusEnum Status { get; set; }
		public object Advocate { get; set; }
		public ClosedBy? ClosedBy { get; set; }

		public TicketStatusChangedModel(Guid conversationId, TicketStatusEnum status, Guid? advocateId, string advocateFirstName = null, decimal? advocateCsat = null, ClosedBy? closedBy = null )
		{
			ConversationId = conversationId;
			Status = status;
			ClosedBy = closedBy;
			Advocate = advocateId != null ? new { Id = advocateId.Value, FirstName =  advocateFirstName, Avatar = advocateFirstName?.Substring(0,1), Csat = advocateCsat.Value } : null;
		}
	}
}