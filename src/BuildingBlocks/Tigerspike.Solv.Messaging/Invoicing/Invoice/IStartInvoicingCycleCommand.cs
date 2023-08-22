using System;

namespace Tigerspike.Solv.Messaging.Invoicing
{
	public interface IStartInvoicingCycleCommand
	{
		/// <summary>
		/// The date that marks the beginning of the new cycle.
		/// This value helps making the command idempotent.
		/// </summary>
		DateTime StartDate { get; set; }
	}
}
