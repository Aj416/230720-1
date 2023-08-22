namespace Tigerspike.Solv.Api.Models
{
	/// <summary>
	/// Change Password Model
	/// </summary>
	public class ChangePasswordModel
	{
		/// <summary>
		/// The old password to be validated.
		/// </summary>
		public string OldPassword { get; set; }

		/// <summary>
		/// The new password to be updated.
		/// </summary>
		public string NewPassword { get; set; }
	}
}