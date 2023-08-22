using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IQuizService
	{
		/// <summary>
		/// Get full quiz for a brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<Quiz> Get(Guid brandId);

		/// <summary>
		/// Get full quiz model for a brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="includeSolutionDetails">Whether or not to include details of correct solution or not</param>
		Task<QuizModel> GetModel(Guid brandId, bool includeSolutionDetails);

		/// <summary>
		/// Check if the quiz answers are acceptable to treat quiz as passed
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="answers">Answers from user</param>
		QuizReviewModel GetQuizReview(Quiz quiz, IEnumerable<QuizAnswerModel> answers);

		/// <summary>
		/// Inserts advocate attempt into database table.
		/// </summary>
		/// <param name="quizId">Quiz Identifier.</param>
		/// <param name="result">The attempt to quiz was pass or fail.</param>
		/// <param name="advocateId">Advocate identifier.</param>
		/// <param name="answers">Answers from user.</param>
		/// <returns>Boolean vaue indicating pass of fail.</returns>
		Task<bool> SaveAttempt(Guid quizId, bool result, Guid advocateId, IEnumerable<QuizAnswerModel> answers);
	}
}
