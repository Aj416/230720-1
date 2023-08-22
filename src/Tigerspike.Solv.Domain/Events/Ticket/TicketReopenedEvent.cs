using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketReopenedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }
		public Guid? AdvocateId { get; }
		public string AdvocateFirstName { get; }
		public decimal AdvocateCsat { get; }
		public string CustomerFirstName { get; }
		public string ReferenceId { get; }

		public bool IsPractice { get; }

		public string SourceName { get; }

		public int? SourceId { get; }

		public DateTime CreatedDate { get; }

		public bool AdvocateBlocked { get; }
		public ReturningCustomerState ReturningCustomerState { get; }
		public NotificationResumptionState NotificationResumptionState { get; }
		public TicketTransportType TransportType { get; }
		public Guid CustomerId { get; set; }

		public TicketReopenedEvent(Guid id, Guid brandId, Guid? advocateId, string advocateFirstName, decimal advocateCsat, string customerFirstName, string referenceId, bool isPractice, string sourceName,
			int? sourceId, DateTime createdDate, ReturningCustomerState returningCustomerState, NotificationResumptionState notificationResumptionState, TicketTransportType transportType, bool advocateBlocked = false, Guid customerId = default)
		{
			TicketId = id;
			BrandId = brandId;
			AdvocateId = advocateId;
			AdvocateCsat = advocateCsat;
			AdvocateFirstName = advocateFirstName;
			CustomerFirstName = customerFirstName;
			ReferenceId = referenceId;
			IsPractice = isPractice;
			SourceName = sourceName;
			SourceId = sourceId;
			CreatedDate = createdDate;
			AdvocateBlocked = advocateBlocked;
			ReturningCustomerState = returningCustomerState;
			NotificationResumptionState = notificationResumptionState;
			TransportType = transportType;
			CustomerId = customerId;
		}
	}
}