using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class ProbingQuestionModel
	{
		public Guid Id { get; set; }
		public string Text { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public IEnumerable<ProbingQuestionOptionModel> Options { get; set; }
	}
}
