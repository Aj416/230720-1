
using System;
using System.Collections.Generic;

namespace Tigerspike.Solv.Application.Models
{
	/// <summary>
	/// Quiz model for the brand
	/// </summary>
	public class QuizResultModel
	{
		/// <summary>
		/// Whether the quiz is passed or not
		/// </summary>
		public bool QuizResult { get; set; }

		/// <summary>
		/// Result description (failure or success message)
		/// </summary>
		public string Message { get; set; }

		public IEnumerable<QuizQuestionReviewModel> QuizReview { get; set; }
		
		public QuizResultModel(bool result, string message)
		{
			QuizResult = result;
			Message = message;
		}

		public QuizResultModel(bool result, string message, IEnumerable<QuizQuestionReviewModel> quizReview)
		{
			QuizResult = result;
			Message = message;
			QuizReview = quizReview;
		}
	}

	public class QuizQuestionReviewModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public IEnumerable<QuizQuestionOptionModel> Options { get; set; }

		public QuizQuestionReviewModel(Guid id, string title, IEnumerable<QuizQuestionOptionModel> options)
		{
			Id = id;
			Title = title;
			Options = options;
		}
	}

	public class QuizReviewModel
	{
		public bool IsQuizPassed { get; set; }
		public IEnumerable<QuizQuestionReviewModel> QuizQuestionReview { get; set; }

		public QuizReviewModel(bool isQuizPassed, IEnumerable<QuizQuestionReviewModel> quizQuestionReview)
		{
			IsQuizPassed = isQuizPassed;
			QuizQuestionReview = quizQuestionReview;
		}
	}
}