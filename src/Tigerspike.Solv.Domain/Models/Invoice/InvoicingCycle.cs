using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Represents a period that invoices can be generated against
	/// </summary>
	public class InvoicingCycle : ICreatedDate
	{
		public InvoicingCycle(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}

		public Guid Id { get; set; }

		/// <summary>
		/// The starting period of this cycle.
		/// </summary>
		public DateTime From { get; set; }

		/// <summary>
		/// The closing period of this cycle.
		/// </summary>
		public DateTime To { get; set; }

		/// <summary>
		/// The creation date of this cycle.
		/// </summary>
		public DateTime CreatedDate { get; set; }
	}
}