using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class DeleteInductionSectionItemCommand : Command
	{
		public Guid BrandId { get; set; }
		public Guid SectionItemId { get; set; }

		public DeleteInductionSectionItemCommand(Guid brandId, Guid sectionItemId)
		{
			BrandId = brandId;
			SectionItemId = sectionItemId;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty && SectionItemId != Guid.Empty;

	}
}