using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Delete Request Model
	/// </summary>
	public class DeleteRequestModel
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
		[Required]
		[StringLength(64, MinimumLength = 64, ErrorMessage = "Invalid key")]
		public string Key { get; set; }
	}
}