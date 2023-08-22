using System;
using System.Collections.Generic;
using MediatR;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands.Invoice
{
	public class CreateBillingDetailsCommand : Command<Unit>
	{
		public Guid BrandId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string VatNumber { get; set; }
		public string CompanyNumber { get; set; }
		public string Address { get; set; }
		public bool IsPlatformOwner { get; set; }

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