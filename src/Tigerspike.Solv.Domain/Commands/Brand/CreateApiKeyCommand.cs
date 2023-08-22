using System;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Extensions;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateApiKeyCommand : Command
	{
		public Guid BrandId { get; set; }
		public string M2M { get; set; }
		public string SDK { get; set; }

		public CreateApiKeyCommand(Guid brandId, string m2mKey, string sdkKey)
		{
			BrandId = brandId;
			M2M = m2mKey;
			SDK = sdkKey;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() =>
			BrandId != Guid.Empty &&
			M2M.IsNotEmpty();

	}
}