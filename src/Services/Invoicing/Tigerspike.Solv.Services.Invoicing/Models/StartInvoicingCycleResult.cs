using System;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	public class StartInvoicingCycleResult
	{
		/// <summary>
		/// Determines if start invoicing cycle was success or failure.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// InvoicingCycle identifier.
		/// </summary>
		public Guid Id { get; set; }
	}
}
