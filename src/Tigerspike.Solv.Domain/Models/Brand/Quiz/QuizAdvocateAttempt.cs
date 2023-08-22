using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Domain.Interfaces;

namespace Tigerspike.Solv.Domain.Models
{
	/// <summary>
	/// Quiz Advocate Attempt for the brand.
	/// </summary>
	public class QuizAdvocateAttempt : ICreatedDate
	{
		/// <summary>
		/// Unique GUID Primary key.
		/// </summary>
		public Guid Id { get; set; }

		/// <inheritdoc/>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Advocate identifier.
		/// </summary>
		public Guid AdvocateId { get; set; }

		/// <summary>
		/// Quiz dentifier.
		/// </summary>
		public Guid QuizId { get; set; }

		/// <summary>
		/// Indicates if the attempt was pass or fail.
		/// </summary>
		public bool Result { get; set; }

		/// <summary>
		/// List of answers.
		/// </summary>
		public List<QuizAdvocateAnswer> Answers { get; private set; }

		/// <summary>
		/// Private constructor for EF only
		/// </summary>
		private QuizAdvocateAttempt() => Answers = new List<QuizAdvocateAnswer>();

		public QuizAdvocateAttempt(Guid advocateId, Guid quizId, bool result)
		{
			AdvocateId = advocateId;
			QuizId = quizId;
			Result = result;
			Answers = new List<QuizAdvocateAnswer>();
		}

		/// <summary>
		/// Add Answers submitted by Advocate.
		/// </summary>
		public void AddAnswers(List<Guid> questionOptions) => Answers.AddRange(questionOptions.Select(optionId => new QuizAdvocateAnswer(Id, optionId)));
	}
}