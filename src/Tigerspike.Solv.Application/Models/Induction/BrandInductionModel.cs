using System.Collections.Generic;
using Tigerspike.Solv.Application.Models.Induction;

namespace Tigerspike.Solv.Application.Models
{
	public class BrandInductionModel
	{
		public IEnumerable<SectionModel> Sections { get; set; } = new List<SectionModel>();
	}
}