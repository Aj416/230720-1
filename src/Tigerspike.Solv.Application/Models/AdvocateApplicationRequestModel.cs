using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Advocate application information when requesting to join the platform
	/// </summary>
	public class AdvocateApplicationRequestModel
	{
		/// <summary>
		/// The country of the advocate.
		/// </summary>
		[Required]
		[StringLength(2, MinimumLength = 2)]
		public string Country { get; set; }

		/// <summary>
		/// The state of the advocate.
		/// </summary>
		[StringLength(20, MinimumLength = 0)]
		public string State { get; set; }

		/// <summary>
		/// The email address of the advocate.
		/// </summary>
		[Required]
		[EmailAddress]
		[StringLength(254)]
		public string Email { get; set; }

		/// <summary>
		/// First name of the advocate.
		/// </summary>
		[Required]
		[StringLength(200)]
		public string FirstName { get; set; }

		/// <summary>
		/// Last name of the advocate.
		/// </summary>
		[Required]
		[StringLength(200)]
		public string LastName { get; set; }

		/// <summary>
		/// Phone number of the advocate.
		/// </summary>
		[Required]
		[Phone]
		[StringLength(30)]
		public string Phone { get; set; }

		/// <summary>
		/// Source where the application heard of Solv.
		/// </summary>
		[Required]
		[StringLength(50)]
		public string Source { get; set; }

		/// <summary>
		/// Google ReCaptcha verification token.
		/// </summary>
		[JsonProperty(PropertyName = "g-recaptcha-response")]
		public string GoogleRecaptchaResponse { get; set; }

		/// <summary>
		/// Whether or not the advocate is old enough to use this platform.
		/// </summary>
		[StringLength(2)]
		public string IsAdult { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to marketing.
		/// </summary>
		[StringLength(2)]
		public string MarketingCheckbox { get; set; }

		/// <summary>
		/// Address of the Advocate
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// City of the Advocate
		/// </summary>
		[StringLength(50)]
		public string City { get; set; }

		/// <summary>
		/// Zip Code of the Advocate
		/// </summary>
		[StringLength(20)]
		public string ZipCode { get; set; }

		/// <summary>
		/// Whether or not the advocate consents to Data Policy.
		/// </summary>
		[StringLength(2)]
		public string DataPolicyCheckbox { get; set; }

		/// <summary>
		/// Password of the Advocate
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Method to sanitize first and last name
		/// </summary>
		public void Sanitize()
		{
			FirstName = !string.IsNullOrEmpty(FirstName) ? new CultureInfo("en").TextInfo.ToTitleCase(FirstName.ToLower().Trim()) : "";
			LastName = !string.IsNullOrEmpty(LastName) ? new CultureInfo("en").TextInfo.ToTitleCase(LastName.ToLower().Trim()) : "";
			Email = Email.ToLowerInvariant();
		}
	}
}