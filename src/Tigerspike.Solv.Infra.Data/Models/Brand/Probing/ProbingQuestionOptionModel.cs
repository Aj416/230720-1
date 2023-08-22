using System;

namespace Tigerspike.Solv.Infra.Data.Models
{
	public class ProbingQuestionOptionModel
	{
		public Guid Id { get; set; }
		public string Text { get; set; }
		public int Order { get; set; }
		public bool RedirectAnswer{get;set;}
	}
}
