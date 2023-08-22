using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Option to select for quiz question
	/// </summary>
	public class QuizQuestionOption
	{
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public bool Correct { get; set; }
		public string Text { get; set; }
		public int Order { get; set; }
		public bool Enabled { get; set; }

		public QuizQuestionOption(){ }

		public QuizQuestionOption(string text, bool correct, int order)
		{
			Id = Guid.NewGuid();
			Correct = correct;
			Text = text;
			Order = order;
		}
	}
}
