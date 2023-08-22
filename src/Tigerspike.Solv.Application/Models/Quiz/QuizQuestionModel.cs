using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Quiz model for the brand
	/// </summary>
	public class QuizQuestionModel
	{
		/// <summary>
		/// The id of the quiz question
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Is question a multi choice or not
		/// </summary>
		public bool IsMultiChoice { get; set; }

		/// <summary>
		/// The question
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The options for the question
		/// </summary>
		public IEnumerable<QuizQuestionOptionModel> Options { get; set; }
	}
}