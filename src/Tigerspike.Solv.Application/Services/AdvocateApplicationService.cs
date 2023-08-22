using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class AdvocateApplicationService : IAdvocateApplicationService
	{
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediator;
		private readonly IAreaRepository _areaRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IQuestionRepository _questionRepository;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IApplicationAnswerRepository _applicationAnswerRepository;
		private readonly IProfileBrandRepository _profileBrandRepository;
		private readonly IUserRepository _userRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IAdvocateBrandRepository _advocateBrandRepository;

		public AdvocateApplicationService(IMapper mapper, IMediatorHandler mediator, IAreaRepository areaRepository,
			IBrandRepository brandRepository,
			IQuestionRepository questionRepository, IAdvocateApplicationRepository advocateApplicationRepository,
			IApplicationAnswerRepository applicationAnswerRepository, IProfileBrandRepository profileBrandRepository,
			IUserRepository userRepository,
			IAdvocateRepository advocateRepository,
			IAdvocateBrandRepository advocateBrandRepository)
		{
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
			_mediator = mediator ??
				throw new ArgumentNullException(nameof(mediator));
			_areaRepository = areaRepository ??
				throw new ArgumentNullException(nameof(areaRepository));
			_brandRepository = brandRepository;
			_questionRepository = questionRepository ??
				throw new ArgumentNullException(nameof(questionRepository));
			_advocateApplicationRepository = advocateApplicationRepository ??
				throw new ArgumentNullException(nameof(advocateApplicationRepository));
			_applicationAnswerRepository = applicationAnswerRepository ??
				throw new ArgumentNullException(nameof(applicationAnswerRepository));
			_profileBrandRepository =
				profileBrandRepository ??
				throw new ArgumentNullException(nameof(profileBrandRepository));
			_userRepository = userRepository ??
				throw new ArgumentNullException(nameof(userRepository));
			_advocateRepository = advocateRepository ??
				throw new ArgumentNullException(nameof(advocateRepository));
			_advocateBrandRepository = advocateBrandRepository ??
				throw new ArgumentNullException(nameof(advocateBrandRepository));
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<BrandInviteModel>> GetBrandAssignments(Guid advocateApplicationId)
		{
			var brands = await _brandRepository.Queryable().Where(b => b.IsPractice == false).ToListAsync();
			var result = _mapper.Map<IEnumerable<BrandInviteModel>>(brands).ToList();
			var assignments = await _advocateApplicationRepository.GetAssignedBrands(advocateApplicationId);
			result.ForEach(x => x.Invited = assignments.Contains(x.Id));
			return result;
		}

		/// <inheritdoc/>
		public async Task<ProfileQuestionsModel> GetAllEnabledQuestions(bool? isEnabled = null)
		{
			var query = _areaRepository.Queryable();

			// false condition so that current profling questions appear and the newer profiling questions remain hidden.
			query = isEnabled.HasValue ? query.Where(q => q.Enabled) : query.Where(q => !q.Enabled);

			query = query.OrderBy(q => q.Order);

			var areas = await query.ToListAsync();

			await query
				.Include(a => a.Questions)
				.ThenInclude(q => q.QuestionType)
				.LoadAsync();

			await query
				.Include(a => a.Questions)
				.ThenInclude(q => q.QuestionOptions)
				.ThenInclude(qo => qo.QuestionDependencies)
				.LoadAsync();

			await query
				.Include(a => a.Questions)
				.ThenInclude(q => q.QuestionOptions)
				.ThenInclude(qo => qo.QuestionOptionDependencies)
				.LoadAsync();

			await query
				.Include(a => a.Questions)
				.ThenInclude(q => q.QuestionOptionCombos)
				.ThenInclude(qoc => qoc.ComboOptions)
				.LoadAsync();

			var returnList = _mapper.Map<List<AreaModel>>(areas);

			foreach (var area in returnList)
			{
				area.Questions = area.Questions.Where(x => x.Enabled).OrderBy(x => x.Order).ToList();
			}

			return new ProfileQuestionsModel { Areas = returnList };
		}

		/// <inheritdoc/>
		public async Task SubmitAnswers(ProfileAnswerModel profileAnswer)
		{
			var newApplicationAnswers = new List<ApplicationAnswer>();

			foreach (var applicationAnswer in profileAnswer.ApplicationAnswers)
			{
				var newApplicationAnswer = _mapper.Map<ApplicationAnswer>(applicationAnswer);
				newApplicationAnswer.Id = Guid.NewGuid();
				newApplicationAnswer.AdvocateApplicationId = profileAnswer.AdvocateApplicationId;

				newApplicationAnswers.Add(newApplicationAnswer);
			}

			await _mediator.SendCommand(
				new SubmitAdvocateApplicationAnswersCommand(profileAnswer.AdvocateApplicationId,
					newApplicationAnswers));
		}

		/// <inheritdoc/>
		public async Task<IList<string>> ValidateAnswers(ProfileAnswerModel profileAnswer)
		{
			var returnList = new List<string>();

			if (!profileAnswer.ApplicationAnswers.Any())
			{
				returnList.Add("No Answers given.");
			}
			else
			{
				for (var loop = 0; loop < profileAnswer.ApplicationAnswers.Count; loop++)
				{
					var applicationAnswer = profileAnswer.ApplicationAnswers[loop];

					if (applicationAnswer.QuestionId == default)
					{
						returnList.Add("Invalid Question ID on item: " + loop);
					}
					else
					{
						var question =
							await _questionRepository.GetFirstOrDefaultAsync(
								predicate: x => x.Id == applicationAnswer.QuestionId, include: x => x
									.Include(y => y.QuestionType));

						if (question == null)
						{
							returnList.Add("Question not found on item: " + loop);
						}
						else
						{
							returnList.AddRange(from answer in applicationAnswer.Answers
												where question.QuestionType.IsSlider && string.IsNullOrEmpty(answer.StaticAnswer)
												select "No answer given when expected on question '" + question.Title + "' item " +
													   loop);
						}
					}

					if (!applicationAnswer.Answers.Any())
					{
						returnList.Add("No answers submitted on item: " + loop);
					}
				}
			}

			return returnList;
		}

		/// <inheritdoc/>
		public async Task InviteAdvocateFromApplication(IEnumerable<Guid> advocateApplicationIds)
		{
			foreach (var id in advocateApplicationIds)
			{
				await _mediator.SendCommand(new InviteAdvocateCommand(id));
			}
		}

		/// <inheritdoc/>
		public async Task DeclineAdvocateApplication(IEnumerable<Guid> advocateApplicationIds)
		{
			foreach (var id in advocateApplicationIds)
			{
				await _mediator.SendCommand(new DeclineAdvocateCommand(id));
			}
		}

		/// <inheritdoc/>
		public async Task<bool> SendDeleteAdvocateApplication(string email) =>
			await _mediator.SendCommand(new SendDeleteAdvocateApplicationCommand(email));

		/// <inheritdoc/>
		public async Task<bool> DeleteAdvocateApplication(string email, string key) =>
			await _mediator.SendCommand(new DeleteAdvocateApplicationCommand(email, key));

		/// <inheritdoc/>
		public async Task<bool> ExportAdvocateApplication(string email)
		{
			var application = await _advocateApplicationRepository.GetFirstOrDefaultAsync(predicate: x =>
				string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase));

			if (application != null)
			{
				var formattedApplication = await GetAdvocateApplication(application.Id);
				formattedApplication.Application.Sanitize();

				var cmd = new ExportAdvocateApplicationCommand(email,
					JsonConvert.SerializeObject(formattedApplication, Formatting.Indented,
						new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

				return await _mediator.SendCommand(cmd);
			}

			return false;
		}

		/// <inheritdoc/>
		public async Task<Guid?> Apply(AdvocateApplicationModel application)
		{
			var cmd = new CreateAdvocateApplicationCommand(application.Country, application.State, application.Email,
				application.FirstName, application.LastName, application.Phone, application.Source, application.IsAdult,
				application.MarketingCheckbox, application.InternalAgent, application.Address, application.City, application.ZipCode,
				application.DataPolicyCheckbox, application.Password);

			var applicationId = await _mediator.SendCommand(cmd);

			return applicationId;
		}

		/// <inheritdoc/>
		public async Task<List<AdvocateApplicationModel>> GetAdvocateApplications()
		{
			var applications = await _advocateApplicationRepository.GetPagedListAsync();
			return _mapper.Map<List<AdvocateApplicationModel>>(applications.Items);
		}

		/// <inheritdoc/>
		public async Task<AdminAdvocateApplicationModel> GetAdvocateApplication(Guid id)
		{
			var questions = await _questionRepository.GetPagedListAsync(include: x => x.Include(y => y.QuestionType));

			var advocateApplication = (await _advocateApplicationRepository.GetFullAdvocateApplication(x => x.Id == id)).SingleOrDefault();

			if (advocateApplication == null)
			{
				return null;
			}

			var adminAdvocateApplication = new AdminAdvocateApplicationModel
			{
				Application = _mapper.Map<AdvocateApplicationModel>(advocateApplication),
				Brands = advocateApplication.BrandAssignments != null
					? advocateApplication.BrandAssignments.Select(ba => ba.Brand.Name)
						.ToList()
					: new List<string>()
			};

			adminAdvocateApplication.Application.ApplicationAnswers = null;

			var areas = await _areaRepository.GetPagedListAsync(orderBy: x => x.OrderBy(y => y.Order));
			foreach (var area in areas.Items)
			{
				var newAdminArea = new AdminAreaModel { Name = area.Title };

				foreach (var applicationAnswer in advocateApplication.ApplicationAnswers
					.Where(x => x.Question.AreaId == area.Id).OrderBy(x => x.Question.Order))
				{
					newAdminArea.Questions.Add(new AdminQuestionModel
					{
						Question = applicationAnswer.Question.Title,
						Answers = _applicationAnswerRepository.GetFormattedApplicationQuestionsAnswers(
							advocateApplication
								.ApplicationAnswers.Where(x =>
									x.QuestionId == applicationAnswer.QuestionId &&
									x.AdvocateApplicationId == applicationAnswer.AdvocateApplicationId).ToList(),
							questions.Items)
					});
				}

				adminAdvocateApplication.Areas.Add(newAdminArea);
			}

			return adminAdvocateApplication;
		}

		/// <inheritdoc/>
		public async Task<bool> Validate(Guid id) => await _advocateApplicationRepository.ExistsAsync(ad =>
			ad.Id == id && ad.ApplicationStatus == AdvocateApplicationStatus.New && !ad.CompletedEmailSent);

		/// <inheritdoc/>
		public async Task<bool> IsEmailInUse(string email) => await _advocateApplicationRepository.IsEmailInUse(email);

		/// <inheritdoc/>
		//public async Task<bool> EnableNewProfiling() =>

		/// <inheritdoc/>
		public async Task<IList<ProfileBrandModel>> GetProfileBrands()
		{
			var profileBrands = await _profileBrandRepository.GetPagedListAsync();

			return _mapper.Map<List<ProfileBrandModel>>(profileBrands.Items.ToList());
		}

		/// <inheritdoc/>
		public async Task<IPagedList<AdvocateApplicationModel>> GetAdminAdvocateApplications(
			AdvocateApplicationStatus status = AdvocateApplicationStatus.New, int pageIndex = 0,
			int pageSize = 25,
			AdminAdvocateApplicationStatusSortBy sortBy = AdminAdvocateApplicationStatusSortBy.CreatedDate,
			SortOrder sortOrder = SortOrder.Desc)
		{
			var advocateApplications = await _advocateApplicationRepository.GetPagedListAsync(
				x => x.ApplicationStatus == status,
				x => x.OrderBy(sortBy, sortOrder),
				pageIndex: pageIndex,
				pageSize: pageSize);

			return _mapper.Map<PagedList<AdvocateApplicationModel>>(advocateApplications);
		}

		/// <inheritdoc />
		public async Task<IList<AdminAdvocateApplicationModel>> GetAllForExport(string country = null,
			bool sanitize = true)
		{
			IPagedList<Guid> applicationsIdList;

			if (!string.IsNullOrEmpty(country))
			{
				applicationsIdList =
					await _advocateApplicationRepository.GetPagedListAsync(selector: x => x.Id,
						predicate: a => a.Country == country.ToUpperInvariant(),
						pageSize: int.MaxValue); // explicitly get all aplications
			}
			else
			{
				applicationsIdList =
					await _advocateApplicationRepository.GetPagedListAsync(selector: x => x.Id,
						pageSize: int.MaxValue); // explicitly get all aplications
			}

			var result = new List<AdminAdvocateApplicationModel>();

			foreach (var id in applicationsIdList.Items)
			{
				var model = await GetAdvocateApplication(id);

				if (sanitize)
				{
					// sanitize data for export
					model.Application.FirstName = null;
					model.Application.LastName = null;
					model.Application.Email = null;
					model.Application.Phone = null;
				}

				result.Add(model);
			}

			return result;
		}

		/// <inheritdoc />
		public async Task<string> GetAllForExport(DateTime? from, DateTime? to)
		{
			var result = await _advocateApplicationRepository.GetExportData(from, to);

			// Select only the required columns
			var advocateApplications = result.Select(aa => new
			{
				aa.Id,
				RegDate = aa.CreatedDate.ToShortDateString(),
				Name = aa.FirstName + " " + aa.LastName,
				aa.Country,
				QUESTIONNAIRE = aa.CompletedEmailSent ? "Yes" : "No",
				aa.Source,
				ApplicationStatus = aa.ApplicationStatus == AdvocateApplicationStatus.New ? "To Review" : aa.ApplicationStatus.ToString(),
				InvitationDate = aa.InvitationDate.HasValue ? aa.InvitationDate.Value.ToShortDateString() : "",
				aa.Email,
				Brand = aa.BrandAssignments.Select(b => b.Brand.Name).Concatenate("|"),
			});

			var header = $"RegDate,Name,Email,Country,Questionnaire,Source,Language,Skills,Status,InvitationDate,Brands";

			var applicationAnswer = await GetCandidateAnswer(result.Select(x => x.Id).ToList());

			string csv;
			using (var sw = new StringWriter())
			{
				// Writing the header row
				await sw.WriteLineAsync(header);
				using (var csvWriter =
					new CsvHelper.CsvWriter(sw, System.Threading.Thread.CurrentThread.CurrentCulture))
				{
					foreach (var advocateApplication in advocateApplications)
					{
						var language = GetDataOnBuisnessValue(applicationAnswer, advocateApplication.Id, QuestionBusinessValue.Language);

						var skills = GetDataOnBuisnessValue(applicationAnswer, advocateApplication.Id, QuestionBusinessValue.Skills);
						// Write the application record
						csvWriter.WriteRecord(
							new
							{
								advocateApplication.RegDate,
								advocateApplication.Name,
								advocateApplication.Email,
								advocateApplication.Country,
								advocateApplication.QUESTIONNAIRE,
								advocateApplication.Source,
								language,
								skills,
								advocateApplication.ApplicationStatus,
								advocateApplication.InvitationDate,
								advocateApplication.Brand
							});

						// flushing the current record so later we can add more columns dynamically (in the next loop)
						await csvWriter.FlushAsync();

						await sw.WriteAsync(Environment.NewLine);
					}
				}

				csv = sw.ToString();
			}
			return csv;
		}

		/// <inheritdoc/>
		public async Task<bool> ValidateProfile(ApplicationAnswerModel model)
		{
			if (model == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, "No Answers given."));
				return false;
			}

			if (model.QuestionId == default)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, $"Invalid Question ID {model.QuestionId}."));
				return false;
			}

			var question = await _questionRepository.GetFirstOrDefaultAsync(
								x => x.Id == model.QuestionId,
								x => x.OrderBy(y => y.Order),
								x => x.Include(y => y.QuestionType));

			if (question == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, $"Question with Id { model.QuestionId} not found."));
				return false;
			}

			if (!model.Answers?.Any() ?? true)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, $"No answers submitted for question with Id { model.QuestionId}."));
				return false;
			}

			if (question.QuestionType.IsSlider && (model.Answers?.Where(a => string.IsNullOrEmpty(a.StaticAnswer)).Any() ?? true))
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, $"No answer given when expected on question { question.Title}."));
				return false;
			}

			if (!question.QuestionType.IsMultiChoice && model.Answers.Count > 1)
			{
				await _mediator.RaiseEvent(new DomainNotification(GetType().Name, $"Multiple choice not applicable for question with Id { model.QuestionId}."));
				return false;
			}

			return true;
		}

		public async Task SubmitProfileAnswers(Guid advocateApplicationId, ApplicationAnswerModel model)
		{
			var applicationAnswer = _mapper.Map<ApplicationAnswer>(model);
			applicationAnswer.AdvocateApplicationId = advocateApplicationId;

			await _mediator.SendCommand(
				new SubmitAdvocateApplicationProfileCommand(applicationAnswer));
		}

		public async Task<IEnumerable<AdvocateApplicationProfileModel>> GetProfileAnswers(Guid advocateApplicationId)
		{
			return await _applicationAnswerRepository.GetAllAsync<AdvocateApplicationProfileModel>(_mapper,
				predicate: pre => pre.AdvocateApplicationId == advocateApplicationId,
				include: inc => inc.Include(i => i.Answers),
				orderBy: ob => ob.OrderBy(o => o.Question.Order));
		}

		public async Task<IList<ApplicationAnswer>> GetCandidateAnswer(List<Guid> advocateApplicationIds)
		{
			return await _applicationAnswerRepository.GetAllAsync(
					predicate: pre => advocateApplicationIds.Contains(pre.AdvocateApplicationId),
					include: inc => inc.Include(i => i.Answers).ThenInclude(x => x.QuestionOption),
					orderBy: ob => ob.OrderBy(o => o.Question.Order));
		}

		public string GetDataOnBuisnessValue(IList<ApplicationAnswer> applicationAnswer, Guid advocateApplicationId, QuestionBusinessValue BusinessValue)
		{
			try
			{
				var advocateAnswer = applicationAnswer
							.Where(x => x.AdvocateApplicationId == advocateApplicationId)
							.SelectMany(x => x.Answers)
							.Where(x => x.QuestionOption.BusinessValue == BusinessValue)
							.Select(x => x.QuestionOption.Text);

				return string.Join("|", advocateAnswer);
			}
			catch
			{
				return string.Empty;
			}
		}

		public async Task<bool> UpdateAdvocateApplicationwithSuperSolver()
		{
			var advocateIds = await _advocateRepository.Queryable().Select(a => a.Id).ToListAsync();

			var advocateApplicationIds = await _advocateApplicationRepository.Queryable().Select(a => a.Id).ToListAsync();

			var missignAdvocateIds = advocateIds.Except(advocateApplicationIds);

			List<AdvocateApplication> advocateApplications = new List<AdvocateApplication>();
			foreach (var advocateid in missignAdvocateIds)
			{
				var user = await _userRepository.GetFirstOrDefaultAsync(predicate: u => u.Id == advocateid);
				var advocateapplication = await _advocateRepository.GetFirstOrDefaultAsync(predicate: a => a.Id == advocateid);
				var application = new AdvocateApplication(user.FirstName, user.LastName, user.Email, user.Phone,
				user.State, user.Country, null, advocateapplication.InternalAgent, null, null, null, advocateid);
				advocateApplications.Add(application);
			}

			var cmd = new UpdateSuperSolverCommand(advocateApplications);
			await _mediator.SendCommand(cmd);

			return true;
		}
	}
}