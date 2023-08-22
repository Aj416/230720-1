using System;

namespace Tigerspike.Solv.Domain.Models.Induction
{
	public class AdvocateSectionItem
	{
		public Guid SectionItemId { get; private set; }

		public Guid AdvocateId { get; private set; }

		public SectionItem SectionItem { get; private set; }

		public Advocate Advocate { get; private set; }

		public AdvocateSectionItem() {}

		public AdvocateSectionItem(Guid advocateId, Guid sectionItemId)
		{
			AdvocateId = advocateId;
			SectionItemId = sectionItemId;
		}
	}
}