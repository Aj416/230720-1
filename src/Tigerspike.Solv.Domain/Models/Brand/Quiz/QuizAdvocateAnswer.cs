using System;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Quiz Advocate Answer for the brand.
	/// </summary>
	public class QuizAdvocateAnswer
	{
		public Guid QuizAdvocateAttemptId { get; set; }

		public Guid QuizQuestionOptionId { get; set; }

		public QuizAdvocateAnswer(Guid quizAdvocateAttemptId, Guid quizQuestionOptionId)
		{
			QuizAdvocateAttemptId = quizAdvocateAttemptId;
			QuizQuestionOptionId = quizQuestionOptionId;
		}
	}
}