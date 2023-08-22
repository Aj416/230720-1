using System;
using System.Collections.Generic;
using Tigerspike.Solv.Core.Commands;

namespace Tigerspike.Solv.Domain.Commands
{
	public class AttemptAdvocateQuizCommand : Command<bool>
	{
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
		public List<Guid> Answers { get; set; }

		/// <summary>
		/// Parameterised constructors.
		/// </summary>
		public AttemptAdvocateQuizCommand(Guid advocateId, Guid quizId, bool result, List<Guid> answers)
		{
			AdvocateId = advocateId;
			QuizId = quizId;
			Result = result;
			Answers = answers;			
		}

		public override bool IsValid() => AdvocateId != Guid.Empty && QuizId != Guid.Empty;
	}
}