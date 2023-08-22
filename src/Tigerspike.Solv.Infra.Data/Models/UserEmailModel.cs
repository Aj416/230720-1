using System;

namespace Tigerspike.Solv.Infra.Data.Models
{

	public class UserEmailModel
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string Email { get; set; }

		public UserEmailModel(Guid id, string firstName, string email)
		{
			Id = id;
			FirstName = firstName;
			Email = email;
		}

	}
}