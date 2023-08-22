using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface ISetBillingDetailsIdCommand
	{
		public Guid BrandId { get; set; }

		public Guid BillingDetailsId { get; set; }
	}
}
