using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Send Delete Request Model
	/// </summary>
	public class SendDeleteRequestModel
	{
		/// <summary>
		/// The email address of the advocate.
		/// </summary>
		[Required]
		[EmailAddress]
		[StringLength(254)]
		public string Email { get; set; }

		/// <summary>
		/// Google ReCaptcha verification token.
		/// </summary>
		[JsonProperty(PropertyName = "g-recaptcha-response")]
		public string GoogleRecaptchaResponse { get; set; }
	}
}