using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models.Induction
{
	public class SectionModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public IEnumerable<SectionItemModel> Items { get; set; }
	}
}