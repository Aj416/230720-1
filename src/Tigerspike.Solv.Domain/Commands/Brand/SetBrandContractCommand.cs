using System;
using MediatR;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class SetBrandContractCommand : Command<Unit>
	{
		public Guid BrandId { get; }
		public string ContractTitle { get; set; }
		public string ContractUrl { get; set; }
		public bool BrandEmployeeCheck { get; set; }

		public SetBrandContractCommand(Guid brandId, string contractTitle, string contractUrl, bool brandEmployeeCheck)
		{
			BrandId = brandId;
			ContractTitle = contractTitle;
			ContractUrl = contractUrl;
			BrandEmployeeCheck = brandEmployeeCheck;
		}

		public override bool IsValid() => BrandId != Guid.Empty;
	}
}