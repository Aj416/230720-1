using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	public class BrandAdvocateResponseConfig
	{
		public Guid Id { get; set; }
		public Guid? BrandId { get; set; }
		public bool IsActive { get; set; }
		public BrandAdvocateResponseType Type { get; set; }
		public string Content { get; set; }
		public int? DelayInSeconds { get; set; }
		public string RelevantTo { get; set; }
		public Guid? ChatActionId { get; set; }
		public int Priority { get; set; }
		public int? AbandonedCount { get; set; }
		public bool? IsAutoAbandoned { get; set; }
		public TicketEscalationReason? EscalationReason { get; set; }
		public UserType? AuthorUserType { get; set;}
		public TicketStatusEnum? StatusOnPosting { get; set; }
	}
}