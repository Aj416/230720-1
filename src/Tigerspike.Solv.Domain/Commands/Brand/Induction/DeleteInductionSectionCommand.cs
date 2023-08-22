using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class DeleteInductionSectionCommand : Command
	{
		public Guid BrandId { get; set; }
		public Guid SectionId { get; set; }

		public DeleteInductionSectionCommand(Guid brandId, Guid sectionId)
		{
			BrandId = brandId;
			SectionId = sectionId;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty && SectionId != Guid.Empty;

	}
}