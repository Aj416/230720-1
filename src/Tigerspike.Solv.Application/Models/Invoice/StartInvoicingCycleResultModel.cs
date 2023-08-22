using System;

namespace Tigerspike.Solv.Application.Models
{
	public class StartInvoicingCycleResultModel
	{
		/// <summary>
		/// Determines if success or failure.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Invoice cycle identifier.
		/// </summary>
		public Guid Id { get; set; }
	}
}
