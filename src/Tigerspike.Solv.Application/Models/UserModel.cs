using System;

namespace Tigerspike.Solv.Application.Models
{
	public class UserModel
	{
		public Guid Id { get; set; }

		public string Email { get; set; }
		public string Avatar { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Phone { get; set; }

		public string DisplayName => $"{FirstName} {LastName}".Trim();

		public bool Enabled { get; set; }

		public DateTime CreatedDate { get; set; }

		public UserModel() { }

		public UserModel(Guid id, string email, string firstName, string lastName, string phone)
		{
			(Id, Email, FirstName, LastName, Phone) = (id, email, firstName, lastName, phone);
			Avatar = firstName != null ? firstName.Substring(0, 1) : null;
		}
	}
}