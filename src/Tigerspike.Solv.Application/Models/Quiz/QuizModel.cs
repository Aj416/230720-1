using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Quiz model for the brand
	/// </summary>
	public class QuizModel
	{

		/// <summary>
		/// The title of the quiz
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The description of the quiz
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Message to show when quiz is failed
		/// </summary>
		public string FailureMessage { get; set; }

		/// <summary>
		/// Message to show when quiz is success
		/// </summary>
		public string SuccessMessage { get; set; }

		/// <summary>
		/// The Allowed Mistakes of the quiz
		/// </summary>
		public int? AllowedMistakes { get; set; }

		/// <summary>
		/// The questions for the quiz
		/// </summary>
		public IEnumerable<QuizQuestionModel> Questions { get; set; }
	}
}