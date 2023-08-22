namespace Tigerspike.Solv.Services.Fraud.Models
{
	public class CustomerModel
	{
		/// <summary>
		/// Customer first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Customer last name.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Customer email id.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Customer Full name.
		/// </summary>
		public string FullName => $"{FirstName} {LastName}".Trim();
	}
}
