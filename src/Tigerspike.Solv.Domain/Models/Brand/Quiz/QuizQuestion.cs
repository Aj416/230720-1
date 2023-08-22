using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Question for the brand quiz
	/// </summary>
	public class QuizQuestion
	{
		public Guid Id { get; set; }
		public Guid QuizId { get; set; }
		public bool IsMultiChoice { get; set; }
		public string Title { get; set; }
		public int Order { get; set; }
		public bool Enabled { get; set; }
		public IEnumerable<QuizQuestionOption> Options { get; set; }

		public QuizQuestion() { }

		public QuizQuestion(string title, bool isMultiChoice, int order, List<QuizQuestionOption> options)
		{
			Id = Guid.NewGuid();
			Title = title;
			IsMultiChoice = isMultiChoice;
			Order = order;
			Options = options;
		}
	}
}
