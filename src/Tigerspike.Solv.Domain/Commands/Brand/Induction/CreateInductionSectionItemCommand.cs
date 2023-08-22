using System;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands.Brand
{
	public class CreateInductionSectionItemCommand : Command
	{
		public Guid BrandId { get; set; }
		public Guid SectionId { get; set; }
		public Guid SectionItemId { get; set; }
		public string Name { get; set; }
		public string Source { get; set; }
		public int Order { get; set; }

		public CreateInductionSectionItemCommand(Guid brandId, Guid sectionId, Guid sectionItemId, string name, string source, int order)
		{
			BrandId = brandId;
			SectionId = sectionId;
			SectionItemId = sectionItemId;
			Name = name;
			Source = source;
			Order = order;
		}

		/// <summary>
		/// Returns if the command is valid.
		/// </summary>
		public override bool IsValid() => BrandId != Guid.Empty && SectionId != Guid.Empty && SectionItemId != Guid.Empty;

	}
}