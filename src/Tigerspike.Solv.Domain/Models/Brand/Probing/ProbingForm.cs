using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Probing form for the brand
	/// </summary>
	public class ProbingForm
	{
		public Guid Id { get; set; }
		public Brand Brand { get; set; }
		public string Title { get; set; }
		public IEnumerable<ProbingQuestion> Questions { get; set; }

		public ProbingForm() { }
	}
}
