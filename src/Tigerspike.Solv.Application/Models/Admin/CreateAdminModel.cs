namespace Tigerspike.Solv.Application.Models.Admin
{
	/// <summary>
	/// A DTO to create admin
	/// </summary>
	public class CreateAdminModel
	{
		/// <summary>
		/// The email id of admin
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// The first name of admin
		/// </summary>
		public string FirstName { get; set; }
		/// <summary>
		/// The last name of admin
		/// </summary>
		public string LastName { get; set; }
		/// <summary>
		/// The password of admin
		/// </summary>
		public string Password { get; set; }
	}
}