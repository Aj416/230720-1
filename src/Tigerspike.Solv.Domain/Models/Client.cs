using System;

namespace Tigerspike.Solv.Domain.Models
{
	public class Client
	{
		/// <summary>
		/// Primary key. Also Foreign key to User
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// The user information associated with this client.
		/// </summary>
		public User User { get; private set; }

		/// <summary>
		/// Foreign key to Brand
		/// </summary>
		public Guid BrandId { get; private set; }

		/// <summary>
		/// Brand assigned to client
		/// </summary>
		public Brand Brand { get; private set; }

		public Client() { }
		public Client(User user, Guid brandId)
		{
			User = user;
			BrandId = brandId;
		}
	}
}
