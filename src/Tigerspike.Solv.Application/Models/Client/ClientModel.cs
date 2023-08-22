using System;

namespace Tigerspike.Solv.Application.Models.Client
{
	public class ClientModel
	{
		public Guid Id { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Phone { get; set; }

		public string DisplayName => $"{FirstName} {LastName}";

		public bool Enabled { get; set; }

		public bool PaymentMethodSetup { get; set; }

		public string PaymentAccountId { get; set; }

		public DateTime CreatedDate { get; set; }
	}
}
