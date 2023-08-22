using System;

namespace Tigerspike.Solv.Application.Models.Induction
{
	public class SectionItemModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public string Source { get; set; }
		public bool? Viewed { get; set; }
	}
}