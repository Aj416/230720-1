using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class BrandIdForInvoicingCommand : IBrandIdForInvoicingCommand
	{
		public bool IsInvoicingEnabled { get; set; }

		public BrandIdForInvoicingCommand(bool isInvoicingEnabled) => IsInvoicingEnabled = isInvoicingEnabled;
	}
}
