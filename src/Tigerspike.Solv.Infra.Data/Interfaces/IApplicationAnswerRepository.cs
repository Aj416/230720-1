using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models.Profile;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface IApplicationAnswerRepository : IRepository<ApplicationAnswer>
	{
		/// <summary>
		/// Formats ApplicationAnswers in a readable format - Questions -&gt; Answers
		/// </summary>
		/// <param name="answers">List of answers to format (Questions/Answers)</param>
		/// <returns>Formatted list of ApplicationAnswers</returns>
		IList<string> GetFormattedApplicationQuestionsAnswers(IList<ApplicationAnswer> answers, IList<Question> questions);

		/// <summary>
		/// Returns whether or not given application has submitted answers
		/// </summary>
		/// <param name="advocateApplicationId"></param>
		/// <returns></returns>
		Task<bool> HasAnswers(Guid advocateApplicationId);

		/// <summary>
		/// Formats ApplicationSkillAnswers in format
		/// </summary>
		/// <param name="answers">List of answers to format (QuestionsOption/Answers)</param>
		/// <returns>Formatted Skill Answers</returns>
		string GetFormattedApplicationSkillAnswers(ICollection<Answer> answers, QuestionOption questionOption);
	}
}