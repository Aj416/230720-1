using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Services.Invoicing.Application.Commands
{
	public class CreateBillingDetailsCommand : Command<Unit>
	{
		/// <summary>
		/// Gets or sets brand id.
		/// </summary>
		public Guid BrandId { get; set; }

		/// <summary>
		/// Gets or sets name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets email.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets VatNumber.
		/// </summary>
		public string VatNumber { get; set; }

		/// <summary>
		/// Gets or sets CompanyNumber.
		/// </summary>
		public string CompanyNumber { get; set; }

		/// <summary>
		/// Gets or sets Address.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets IsPlatformOwner.
		/// </summary>
		public bool IsPlatformOwner { get; set; }

		/// <summary>
		/// Parameterised constructor.
		/// </summary>
		/// <param name="brand">Brand id.</param>
		/// <param name="name">Name</param>
		/// <param name="email">Email</param>
		/// <param name="vatNumber">VatNumber</param>
		/// <param name="companyNumber">CompanyNumber</param>
		/// <param name="address">Address</param>
		/// <param name="isPlatformOwner">IsPlatformOwner</param>
		public CreateBillingDetailsCommand(Guid brandId, string name, string email, string vatNumber, string companyNumber, string address, bool isPlatformOwner)
		{
			BrandId = brandId;
			Name = name;
			Email = email;
			VatNumber = vatNumber;
			CompanyNumber = companyNumber;
			Address = address;
			IsPlatformOwner = isPlatformOwner;
		}

		public override bool IsValid() => BrandId != Guid.Empty && Name.IsNotEmpty() && Email.IsNotEmpty();
	}
}
