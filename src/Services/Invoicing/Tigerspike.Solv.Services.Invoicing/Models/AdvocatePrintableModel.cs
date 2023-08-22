namespace Tigerspike.Solv.Services.Invoicing.Models
{
	/// <summary>
	/// Advocate's data to use on printable documents
	/// </summary>
	public class AdvocatePrintableModel
	{
		/// <summary>
		/// Advocate first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Advocate last name.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Advocate phone number.
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Advocate email id.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Advocate country code.
		/// </summary>
		public string CountryCode { get; set; }
	}
}
