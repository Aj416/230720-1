using System;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	public class BillingDetails : ICreatedDate
	{
		/// <summary>
		/// Constructor to please EF.
		/// </summary>
		private BillingDetails() { }

		public BillingDetails(string name, string email, string vatNumber, string companyNumber, string address, bool isPlatformOwner)
		{
			Name = name;
			Email = email;
			VatNumber = vatNumber;
			CompanyNumber = companyNumber;
			Address = address;
			IsPlatformOwner = isPlatformOwner;
		}

		public Guid Id { get; private set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		/// Vat number
		/// </summary>
		public string VatNumber { get; private set; }

		/// <summary>
		/// Company number
		/// </summary>
		public string CompanyNumber { get; private set; }

		/// <summary>
		/// Address
		/// </summary>
		public string Address { get; private set; }

		/// <summary>
		/// True if this record contains details of platform's owner
		/// </summary>
		public bool IsPlatformOwner { get; private set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }
	}
}