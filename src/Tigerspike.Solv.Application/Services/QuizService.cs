using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class QuizService : IQuizService
	{
		private readonly IQuizRepository _quizRepository;
		private readonly IMediatorHandler _mediator;
		private readonly IMapper _mapper;
		public QuizService(
			IQuizRepository quizRepository, IMediatorHandler mediator, IMapper mapper)
		{
			_mediator = mediator ??
			throw new ArgumentNullException(nameof(mediator));
			_quizRepository = quizRepository;
			_mapper = mapper;
		}


		/// <inheritdoc/>
		public async Task<Quiz> Get(Guid brandId)
		{
			return await _quizRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Brand.Id == brandId,
				include: inc => inc
					.Include(x => x.Questions).ThenInclude(x => x.Options)
			);
		}

		/// <inheritdoc/>
		public async Task<QuizModel> GetModel(Guid brandId, bool includeSolutionDetails)
		{
			var quiz = await _quizRepository.GetFirstOrDefaultAsync(
				predicate: x => x.Brand.Id == brandId, 
				include: x => x.Include(q => q.Questions)
				.ThenInclude(q => q.Options));

			var quizModel = _mapper.Map<QuizModel>(quiz);

			if (quizModel != null && includeSolutionDetails == false)
			{
				foreach (var q in quizModel.Questions)
				{
					foreach (var o in q.Options)
					{
						o.Correct = null;
					}
				}
			}

			return quizModel;
		}

		/// <inheritdoc/>
		public QuizReviewModel GetQuizReview(Quiz quiz, IEnumerable<QuizAnswerModel> answers)
		{
			var maxScore = quiz.Questions.Count();
			var threshold = maxScore - quiz.AllowedMistakes;

			var answersPerQuestion = answers.ToDictionary(x => x.Id, x => x);
			var score = quiz.Questions
				.Select(x => GetQuestionScore(x, answersPerQuestion.GetValueOrDefault(x.Id)))
				.Sum();

			var quizQuestionReview = quiz.Questions.OrderBy(x => x.Order).Select(x => new QuizQuestionReviewModel(
				x.Id, x.Title, GetAdvocateQuiz(x, answersPerQuestion.GetValueOrDefault(x.Id))));

			return new QuizReviewModel(score >= threshold, quizQuestionReview);
		}

		public async Task<bool> SaveAttempt(Guid advocateId, bool result, Guid quizId, IEnumerable<QuizAnswerModel> answers)
		=> await _mediator.SendCommand(new AttemptAdvocateQuizCommand(advocateId, quizId, result, answers.SelectMany(a => a.Answers).ToList()));

		private int GetQuestionScore(QuizQuestion question, QuizAnswerModel answer)
		{
			var answers = answer?.Answers.OrderBy(x => x).ToList() ?? new List<Guid>();
			var correctOptions = question.Options
				.Where(x => x.Correct)
				.Select(x => x.Id)
				.OrderBy(x => x)
				.ToList();

			return answers.SequenceEqual(correctOptions) ? 1 : 0;
		}

		private List<QuizQuestionOptionModel> GetAdvocateQuiz(QuizQuestion question, QuizAnswerModel answer)
		{
			bool? b = null;
			var answers = answer?.Answers.OrderBy(x => x).ToList() ?? new List<Guid>();
			var advocateOptions = question.Options.OrderBy(x => x.Order)
				.Select(x => new QuizQuestionOptionModel(x.Id, x.Text, answers.Contains(x.Id) ? (x.Correct ? true : false) : b));

			return advocateOptions.ToList();
		}
	}
}