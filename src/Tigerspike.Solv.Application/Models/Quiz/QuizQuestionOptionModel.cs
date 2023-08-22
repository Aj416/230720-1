using System;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Quiz option model for the brand
	/// </summary>
	public class QuizQuestionOptionModel
	{
		/// <summary>
		/// The id of the quiz question option
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The option text
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// The option Correct
		/// </summary>
		public bool? Correct { get; set; }

		public QuizQuestionOptionModel(Guid id, string text, bool? correct)
		{
			Id = id;
			Text = text;
			Correct = correct;
		}
	}
}