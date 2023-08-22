using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Question for the probing form
	/// </summary>
	public class ProbingQuestion
	{
		public Guid Id { get; set; }
		public Guid ProbingFormId { get; set; }
		public string Text { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public IEnumerable<ProbingQuestionOption> Options { get; set; }

		public ProbingQuestion() { }
	}
}
