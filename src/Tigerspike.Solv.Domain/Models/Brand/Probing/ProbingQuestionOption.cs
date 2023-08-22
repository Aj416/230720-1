using System;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Option to select for probing question
	/// </summary>
	public class ProbingQuestionOption
	{
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public string Text { get; set; }
		public TicketFlowAction? Action { get; set; }
		public int Order { get; set; }
		public string Value { get; set; }

		public bool RedirectAnswer{get;set;}

		public ProbingQuestionOption() { }
	}
}
