using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Billing details for the invoice
	/// </summary>
	public class BillingDetailsPrintableModel
	{
		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Vat number
		/// </summary>
		public string VatNumber { get; set; }

		/// <summary>
		/// Company number
		/// </summary>
		public string CompanyNumber { get; set; }

		/// <summary>
		/// Address
		/// </summary>
		public IEnumerable<string> AddressLines { get; set; }
	}
}