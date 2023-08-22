using System;
using Tigerspike.Solv.Core.Events;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Events.Ticket
{
	public class TicketSolvedEvent : Event
	{
		public Guid TicketId { get; }

		public Guid BrandId { get; }

		public string ReferenceId { get; }

		public bool IsPractice { get; }

		public string SourceName { get; }

		public Guid AdvocateId { get; }

		public string AdvocateFirstName { get; }
		public decimal AdvocateCsat { get; }

		public TicketTransportType TransportType { get; }

		public int WaitMinutesToClose { get; }
		public string ThreadId { get; set; }
		public Guid CustomerId { get; set; }

		public TicketSolvedEvent(Guid id, Guid brandId, string referenceId, string threadId, bool isPractice, string sourceName,
			Guid advocateId, string advocateFirstName, decimal advocateCsat,
			TicketTransportType transportType, int waitMinutesToClose, Guid customerId)
		{
			TicketId = id;
			BrandId = brandId;
			ReferenceId = referenceId;
			ThreadId = threadId;
			IsPractice = isPractice;
			SourceName = sourceName;
			AdvocateId = advocateId;
			AdvocateFirstName = advocateFirstName;
			AdvocateCsat = advocateCsat;
			TransportType = transportType;
			WaitMinutesToClose = waitMinutesToClose;
			CustomerId = customerId;
		}
	}
}