namespace Tigerspike.Solv.Services.Fraud.Domain
{
	public class Customer
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
		/// Customer IP Address.
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="firstName">Customer first name.</param>
		/// <param name="lastName">Customer last name.</param>
		/// <param name="email">Customer email id.</param>
		/// <param name="ipAddress">Customer IP address.</param>
		public Customer(string firstName, string lastName, string email, string ipAddress = "")
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			IpAddress = ipAddress;
		}
	}
}
