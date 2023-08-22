using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class User : ICreatedDate, IModifiedDate
	{
		/// <summary>
		/// The user identifier.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// User's first name
		/// </summary>
		public string FirstName { get; private set; }

		/// <summary>
		/// User's last name
		/// </summary>
		public string LastName { get; private set; }

		/// <summary>
		/// User's full name (calculated)
		/// </summary>
		public string FullName => $"{FirstName} {LastName}";

		/// <summary>
		/// The user email
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		/// The user alternate email
		/// </summary>
		public string AlternateEmail { get; private set; }

		/// <summary>
		/// Users' phone number
		/// </summary>
		public string Phone { get; private set; }

		/// <summary>
		/// The country of the advocate.
		/// </summary>
		public string Country { get; private set; }

		/// <summary>
		/// The state of the advocate.
		/// </summary>
		public string State { get; private set; }

		/// <summary>
		/// Indicates whether the user is enabled.
		/// </summary>
		public bool Enabled { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <inheritdoc/>
		public DateTime ModifiedDate { get; set; }

		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private User() { }

		public User(Guid id, string firstName, string lastName)
		{
			Id = id;
			FirstName = firstName;
			LastName = lastName;
			Enabled = true; // It is enabled by default.
		}

		public User(Guid id, string firstName, string lastName, string email, string phone)
		{
			Id = id;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Phone = phone;
			Enabled = true; // It is enabled by default.
		}

		public User(Guid id, string firstName, string lastName, string email, string phone, string country, string state)
		{
			Id = id;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Phone = phone;
			Country = country;
			State = state;
			Enabled = true; // It is enabled by default.
		}

		public void SetEnabled(bool value) => Enabled = value;
		public void SetPhone(string phone) => Phone = phone;

		public void Block() => Enabled = false;
		public void Unblock() => Enabled = true;

		public void ChangeName(string firstName, string lastName)
		{
			FirstName = firstName;
			LastName = lastName;
		}
	}
}