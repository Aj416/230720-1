using System.ComponentModel.DataAnnotations;

namespace Tigerspike.Solv.Api.Models.Advocate
{
	/// <summary>
	/// The request model to create an advocate
	/// </summary>
	public class AdvocateCreateModel
	{
		/// <summary>
		/// The advocate application token that passed in the email.
		/// </summary>
		[Required]
		public string Token { get; set; }

		/// <summary>
		/// The password to be used with the advocate account
		/// </summary>
		[Required]
		public string Password { get; set; }
	}
}
