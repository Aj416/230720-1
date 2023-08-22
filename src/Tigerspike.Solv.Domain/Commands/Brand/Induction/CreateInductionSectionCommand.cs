using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateInductionSectionCommand : Command
	{
		public Guid BrandId { get; set; }
		public Guid SectionId { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }

		public CreateInductionSectionCommand(Guid brandId, Guid sectionId, string name, int order)
		{
			BrandId = brandId;
			SectionId = sectionId;
			Name = name;
			Order = order;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty && SectionId != Guid.Empty;

	}
}