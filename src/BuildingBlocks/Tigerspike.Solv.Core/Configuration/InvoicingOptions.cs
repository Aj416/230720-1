using Tigerspike.Solv.Core.Models;

namespace Tigerspike.Solv.Core.Configuration
{
	/// <summary>
	/// Holds configuration about Invoicing
	/// </summary>
	public class InvoicingOptions
	{
		public const string SectionName = "Invoicing";

		/// <summary>
		/// How often to generate the invoices.
		/// </summary>
		public Periodicity Periodicity { get; set; }

		/// <summary>
		/// The currency that we use for payments.
		/// </summary>
		public string CurrencyCode { get; set; }
	}

}