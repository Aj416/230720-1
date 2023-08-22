using System;

namespace Tigerspike.Solv.Services.Invoicing.Models
{
	/// <summary>
	/// Invoicing cycle details model
	/// </summary>
	public class InvoicingCyclePrintableModel
	{
		/// <summary>
		/// The system identifier of the invoice
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// From date
		/// </summary>
		public DateTime From { get; set; }

		/// <summary>
		/// To date
		/// </summary>
		public DateTime To { get; set; }
	}
}
