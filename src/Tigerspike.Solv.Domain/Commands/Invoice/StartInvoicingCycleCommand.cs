using System;
using System.Security.Permissions;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	/// <summary>
	/// Initiate a new invoicing cycle, that can consequently lead to invoice generation
	/// </summary>
	public class StartInvoicingCycleCommand
	{
		/// <summary>
		/// The date that marks the beginning of the new cycle.
		/// This value helps making the command idempotent.
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Empty constructor, so MassTransit can deserialize the message
		/// </summary>
		protected StartInvoicingCycleCommand()
		{

		}

		public StartInvoicingCycleCommand(DateTime startDate)
		{
			StartDate = startDate;
		}

		public bool IsValid() => StartDate <= DateTime.UtcNow && StartDate > DateTime.MinValue;
	}

	public class StartInvoicingCycleResult
	{
		public bool Success { get; set; }

		public Guid Id { get; set; }
	}
}