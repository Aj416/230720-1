using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Quiz for the brand
	/// </summary>
	public class Quiz
	{
		public Guid Id { get; set; }
		public Brand Brand { get; set; }
		public string Title { get; set; }
		public string FailureMessage { get; set; }
		public string SuccessMessage { get; set; }
		public string Description { get; set; }
		public int AllowedMistakes { get; set; }
		public IEnumerable<QuizQuestion> Questions { get; set; }

		public Quiz() { }

		public Quiz(string title, string description, string failureMessage, string successMessage, int allowedMistakes, List<QuizQuestion> questions)
		{
			Id = Guid.NewGuid();
			Title = title;
			FailureMessage = failureMessage;
			SuccessMessage = successMessage;
			Description = description;
			AllowedMistakes = allowedMistakes;
			Questions = questions;
		}
	}
}
