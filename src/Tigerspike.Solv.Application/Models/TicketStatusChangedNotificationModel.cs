using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Models
{
	public class TicketStatusChangedNotificationModel
	{
		public Guid TicketId { get; set; }
		public TicketStatusEnum ToStatus { get; set; }
		public Guid? AdvocateId { get; set; }
		public string AdvocateFirstName { get; set; }
		public decimal? AdvocateCsat { get; set; }
		public Guid CustomerId { get; set; }
		public ClosedBy? ClosedBy { get; set; }
		public TicketTagStatus? TagStatus { get; set; }
	}
}