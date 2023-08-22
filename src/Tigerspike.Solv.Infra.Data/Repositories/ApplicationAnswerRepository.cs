using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Infra.Data.Repositories
{
	public class ApplicationAnswerRepository : Repository<ApplicationAnswer>, IApplicationAnswerRepository
	{
		private IQuestionRepository _questionRepository;

		public ApplicationAnswerRepository(SolvDbContext context, IQuestionRepository questionRepository) : base(context)
		{
			_questionRepository = questionRepository;
		}

		/// <inheritdoc/>
		public IList<string> GetFormattedApplicationQuestionsAnswers(IList<ApplicationAnswer> answers, IList<Question> questions)
		{
			var returnList = new List<string>();

			foreach (var question in answers)
			{
				var questionType = questions.FirstOrDefault(x => x.Id == question.QuestionId)?.QuestionType.Name;

				foreach (var answer in question.Answers)
				{
					if (questionType == QuestionType.SingleChoice.ToString() || questionType == QuestionType.MultiChoice.ToString())
					{
						returnList.Add(answer.QuestionOption.Text);
					}
					if (questionType == QuestionType.SingleSlider.ToString() || questionType == QuestionType.MultiSlider.ToString())
					{
						int.TryParse(answer.StaticAnswer, out var sliderValue);
						returnList.Add(answer.QuestionOption.Text + ": " + (Slider)sliderValue);
					}
					else if (questionType == QuestionType.TagInput.ToString())
					{
						returnList.Add(answer.StaticAnswer);
					}
				}
			}

			return returnList;
		}

		/// <inheritdoc/>
		public Task<bool> HasAnswers(Guid advocateApplicationId) => Queryable().AnyAsync(x => x.AdvocateApplicationId == advocateApplicationId);

		/// <inheritdoc/>
		public string GetFormattedApplicationSkillAnswers(ICollection<Answer> answers, QuestionOption questionOption)
		{
			return answers.Where(x => x.QuestionOptionId == questionOption.Id)
				.Select(x => x.StaticAnswer == "null" ? "Yes" : Enum.Parse<Slider>(x.StaticAnswer).ToString())
				.FirstOrDefault();
		}

		private enum Slider
		{
			None = 0,
			Beginner = 1,
			Intermediate = 2,
			Expert = 3
		}

		private enum QuestionType
		{
			SingleChoice,
			SingleSlider,
			MultiChoice,
			MultiSlider,
			TagInput
		}
	}
}