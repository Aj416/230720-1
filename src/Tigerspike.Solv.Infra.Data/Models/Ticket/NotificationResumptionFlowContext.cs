using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class NotificationResumptionFlowContext
	{
		public Guid TicketId { get; set; }
		public Guid BrandId { get; set; }
		public bool Canceled { get; set; }

		public NotificationResumptionFlowContext()
		{

		}

		public NotificationResumptionFlowContext(Guid ticketId, Guid brandId)
		{
			TicketId = ticketId;
			BrandId = brandId;
		}
	}
}