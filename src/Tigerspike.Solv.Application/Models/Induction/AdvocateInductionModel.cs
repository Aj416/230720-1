using System.Collections.Generic;
using System.Linq;

namespace Tigerspike.Solv.Application.Models.Induction
{
	public class AdvocateInductionModel
	{
		public bool Completed => Sections != null && Sections.All(s => s.Items != null && s.Items.All(si => si.Viewed == true));

		public bool GuidelineModalAgreed { get; set; }

		public List<SectionModel> Sections { get; set; }
	}
}