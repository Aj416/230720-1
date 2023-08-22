using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class ReturningCustomerFlowContext
	{
		public Guid TicketId { get; set; }
		public Guid BrandId { get; set; }
		public bool Canceled { get; set; }

		public ReturningCustomerFlowContext()
		{

		}

		public ReturningCustomerFlowContext(Guid ticketId, Guid brandId)
		{
			TicketId = ticketId;
			BrandId = brandId;
		}
	}
}