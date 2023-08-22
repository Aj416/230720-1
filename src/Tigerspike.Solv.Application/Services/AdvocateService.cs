using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Admin;
using Tigerspike.Solv.Application.Models.Induction;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Profile;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class AdvocateService : IAdvocateService
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IAdvocateBrandRepository _advocateBrandRepository;
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediator;
		private readonly IBrandRepository _brandRepository;
		private readonly IAreaRepository _areaRepository;
		private readonly IApplicationAnswerRepository _applicationAnswerRepository;
		private readonly IQuestionRepository _questionRepository;

		public AdvocateService(
			IBrandRepository brandRepository,
			IAdvocateApplicationRepository advocateApplicationRepository,
			IAdvocateRepository advocateRepository,
			IAdvocateBrandRepository advocateBrandRepository,
			IAuthenticationService authenticationService,
			IMapper mapper,
			IAreaRepository areaRepository,
			IQuestionRepository questionRepository,
			IApplicationAnswerRepository applicationAnswerRepository,
			IMediatorHandler mediator)
		{
			_brandRepository = brandRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_advocateApplicationRepository = advocateApplicationRepository ??
				throw new ArgumentNullException(nameof(advocateApplicationRepository));
			_advocateRepository = advocateRepository ??
				throw new ArgumentNullException(nameof(advocateRepository));
			_advocateBrandRepository = advocateBrandRepository ??
				throw new ArgumentNullException(nameof(advocateBrandRepository));
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
			_mediator = mediator ??
				throw new ArgumentNullException(nameof(mediator));
			_areaRepository = areaRepository ??
				throw new ArgumentNullException(nameof(areaRepository));
			_questionRepository = questionRepository ??
			throw new ArgumentNullException(nameof(questionRepository));
			_applicationAnswerRepository = applicationAnswerRepository ??
			throw new ArgumentNullException(nameof(applicationAnswerRepository));
			_authenticationService =
				authenticationService ??
				throw new ArgumentNullException(nameof(authenticationService));
		}

		/// <inheritdoc/>
		public async Task<AdvocateModel> FindAsync(Guid advocateId, Guid? brandId = null)
		{
			var advocate = await _advocateRepository.GetFirstOrDefaultAsync(i => i,
				pr => pr.Id == advocateId && (brandId == null || pr.Brands.Any(br => br.BrandId == brandId)),
				include: src => src.Include(u => u.User).Include(b => b.BlockHistory).ThenInclude(bb => bb.Brand));

			// TODO: if you have a better idea to get the advocate brand object with the previous query, please do so :)
			var advocateBrand = await _advocateBrandRepository.FindAsync(advocateId, brandId);

			var advocateDto = _mapper.Map<AdvocateModel>(advocate);
			if (advocateDto != null)
			{
				advocateDto.Csat = advocateBrand?.Csat ?? advocate.Csat;
			}

			return advocateDto;
		}

		/// <inheritdoc/>
		public async Task<bool> ExistsAsync(Guid advocateId, Guid? brandId = null)
		{
			var advocate = await _advocateRepository.GetFirstOrDefaultAsync(i => (Guid?)i.Id,
				pr => pr.Id == advocateId && (brandId == null || pr.Brands.Any(br => br.BrandId == brandId)));

			return advocate != null;
		}

		/// <inheritdoc/>
		public async Task Create(string token, string password)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			if (password == null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			var application = await _advocateApplicationRepository.GetFirstOrDefaultAsync(i => i,
				pr => pr.Token == token);

			if (application == null)
			{
				throw new ServiceInvalidOperationException($"The application was not found.");
			}

			await _mediator.SendCommand(new CreateAdvocateIdentityCommand(application.Id, application.FirstName,
				application.LastName, application.Email, application.Phone, application.Country, application.Source, application.InternalAgent, password));
		}

		/// <inheritdoc/>
		public Task SetVideoWatched(Guid advocateId) => _mediator.SendCommand(new SetVideoWatchedCommand(advocateId));

		/// <inheritdoc/>
		public async Task<bool> ValidateToken(string token)
		{
			// TODO: This should be checked against the expiry date, once we have a process to renew the token and send it back to the advocate
			var application = await _advocateApplicationRepository.GetFirstOrDefaultAsync(i => i,
				pr => pr.Token == token);

			return application != null;
		}

		/// <inheritdoc/>
		public Task StartPractice(Guid advocateId) =>
			_mediator.SendCommand(new StartAdvocatePracticeCommand(advocateId));

		/// <inheritdoc/>
		public Task AcceptBrandAgreement(Guid advocateId, Guid brandId) =>
			_mediator.SendCommand(new AcceptBrandAgreementCommand(advocateId, brandId));

		/// <inheritdoc/>
		public Task AgreeToContract(Guid advocateId, Guid brandId) =>
			_mediator.SendCommand(new AgreeToContractCommand(advocateId, brandId));

		/// <inheritdoc/>
		public Task EnableBrand(Guid advocateId, Guid brandId) =>
			_mediator.SendCommand(new EnableBrandCommand(advocateId, brandId));

		/// <inheritdoc/>
		public Task DisableBrand(Guid advocateId, Guid brandId) =>
			_mediator.SendCommand(new DisableBrandCommand(advocateId, brandId));

		/// <inheritdoc />
		public async Task<AdvocateStatisticByStatusModel> GetStatisticsByStatusForAll()
		{
			return new AdvocateStatisticByStatusModel
			{
				Total = await _advocateRepository.CountAsync(),
				// Idle = we don't know how to count those yet
				Blocked = await _advocateRepository.CountAsync(x => x.User.Enabled == false),
				// Authorized = we don't know how to count those yet
			};
		}

		/// <inheritdoc />
		public async Task<AdvocateStatisticByStatusModel> GetStatisticsByStatusForBrand(Guid brandId)
		{
			return new AdvocateStatisticByStatusModel
			{
				Total = await _advocateBrandRepository.GetTotalCountForBrand(brandId),
				Idle = await _advocateBrandRepository.GetIdleCountByBrand(brandId),
				Blocked = await _advocateBrandRepository.GetBlockedCountByBrand(brandId),
				Authorized = await _advocateBrandRepository.GetAuthorizedCountByBrand(brandId),
			};
		}

		/// <inheritdoc />
		public async Task<AdvocateSearchModel> GetAdvocateForSearch(Guid advocateId) =>
			_mapper.Map<AdvocateSearchModel>(await _advocateRepository.GetFullAdvocateAsync(a => a.Id == advocateId));

		/// <inheritdoc />
		public async Task<AdvocateInductionModel> GetInduction(Guid advocateId, Guid brandId)
		{
			var sections = await _brandRepository.GetInductionSections(brandId);

			var responses = await _advocateRepository.GetInductionSectionItems(advocateId, brandId);

			var guidelineAgreed = await _advocateBrandRepository.FindAsync(advocateId, brandId);

			var advocateInductionModel = new AdvocateInductionModel
			{
				Sections = _mapper.Map<List<SectionModel>>(sections),
				GuidelineModalAgreed = guidelineAgreed.GuidelineAgreed
			};

			// Set the status of the responses
			foreach (var sectionItemModel in advocateInductionModel.Sections.SelectMany(sectionModel =>
					sectionModel.Items))
			{
				sectionItemModel.Viewed = responses.Any(r => r.SectionItemId == sectionItemModel.Id);
			}

			return advocateInductionModel;
		}

		/// <inheritdoc />
		public Task MarkInductionItem(Guid advocateId, Guid brandId, Guid itemId) =>
			_mediator.SendCommand(new MarkInductionItemCommand(advocateId, brandId, itemId));

		/// <inheritdoc />
		public Task SetGuideLine(Guid advocateId, Guid brandId) =>
			_mediator.SendCommand(new SetGuideLineCommand(advocateId, brandId));

		/// <inheritdoc/>
		public async Task MarkQuizAsPassed(Guid advocateId, Guid brandId) => await _mediator.SendCommand(new CompleteInductionCommand(advocateId, brandId));

		/// <inheritdoc />
		public async Task<(bool paymentMethodSetup, bool emailVerified, string paymentAccountId)> UpdatePaymentMethodStatus(Guid advocateId)
		{
			await _mediator.SendCommand(new UpdateAdvocatePaymentAccountCommand(advocateId));
			var advocate = await _advocateRepository.FindAsync(advocateId);
			return (advocate.PaymentMethodSetup, advocate.PaymentEmailVerified, advocate.PaymentAccountId);
		}

		/// <inheritdoc/>
		public async Task<string> GetAllAdvocate()
		{
			var brands = await _brandRepository.Queryable().ToListAsync();
			//Get all Advocate data
			var advocates = await _advocateRepository.GetAllAsync(
				include: src => src.Include(i => i.User).Include(i => i.Brands).ThenInclude(i => i.Brand));

			var result = await _advocateApplicationRepository.GetFullAdvocateApplication();

			// Select only the required columns
			var advocateApplications = result.Select(aa => new
			{
				aa.Id,
				aa.FirstName,
				aa.LastName,
				aa.Country,
				aa.Address,
				aa.City,
				aa.ZipCode,
				aa.Email,
				aa.Phone,
				AplicationDate = aa.CreatedDate.ToShortDateString(),
				CompletedProfiling = aa.CompletedEmailSent ? "Yes" : "No",
				InvitationDate = aa.InvitationDate.HasValue ? aa.InvitationDate.Value.ToShortDateString() : "",
				LastInvitationDate = aa.LastInvitationDate.HasValue ? aa.LastInvitationDate.Value.ToShortDateString() : "",
				AccountCreated = aa.ApplicationStatus == AdvocateApplicationStatus.AccountCreated ? "Yes" : "No",
			});

			//A dictionary to lookup advocateApplications by advocate id
			var advocateApplicationsDict = advocateApplications.ToDictionary(a => (a.Id), x => x);
			// Get all the association between Advocates and Brands
			var advocateBrands = await _advocateBrandRepository.GetAllForCsvExport();
			// A dictionary to lookup advocate brand by advocate id and brand Id
			var advAppDict = advocateBrands.ToDictionary(d => (d.AdvocateId, d.BrandId), x => x);
			// The header of the csv file containing the application columns
			var header = $"{nameof(AdvocateApplication.Id)},{nameof(AdvocateApplication.FirstName)},{nameof(AdvocateApplication.LastName)},{nameof(AdvocateApplication.Country)},{nameof(AdvocateApplication.Address)},{nameof(AdvocateApplication.City)},{nameof(AdvocateApplication.ZipCode)},{nameof(AdvocateApplication.Email)},MobileNumber, AplicationDate, CompletedProfiling, InvitationDate, LastInvitationDate, AccountCreated,SolverLevel,PlatformBlocked,RemovedBrands,BrandsInvitedTo,";
			// Brands records as pivot columns
			var pivotBrands = brands
				.Select(b => $"{b.Name}_Authorized, {b.Name}_ContractAccepted, {b.Name}_Inducted")
				.Aggregate((a, b) => $"{a}, {b}");

			var areas = await _areaRepository.GetAllAsync(orderBy: x => x.OrderBy(y => y.Order), predicate: x => x.Title != Area.Skills);
			var areasTitle = "," + areas.Select(a => a.Title).Aggregate((a, b) => $"{a} ,{b}");
			// Get Answers of each skills level
			var listOfAnswers = await GetListOfAnswers(result, areas);
			var answersDict = listOfAnswers.ToDictionary(x => (x.AdvocateId, x.AreaId), x => x);

			//Get Header for all the skils
			var skillLists = await GetListOfSkillsQuestionOptions();
			var skillsTitle = "," + skillLists.Select(a => a.Text).Aggregate((a, b) => $"{a} ,{b}");

			//Get answer for all skils
			var listOfSkillsAnswer = GetListOfSkilsAnswers(result, skillLists);
			var skillDict = listOfSkillsAnswer.ToDictionary(x => (x.AdvocateId, x.QuestionOptionId), x => x);

			string csv;
			using (var sw = new StringWriter())
			{
				// Writing the header row
				await sw.WriteLineAsync(header + pivotBrands + areasTitle + skillsTitle);
				using (var csvWriter =
					new CsvHelper.CsvWriter(sw, System.Threading.Thread.CurrentThread.CurrentCulture))
				{
					foreach (var advocate in advocates)
					{
						advocateApplicationsDict.TryGetValue((advocate.Id), out var adv);
						// Write the application record
						csvWriter.WriteRecord(adv);

						// flushing the current record so later we can add more columns dynamically (in the next loop)
						await csvWriter.FlushAsync();

						await sw.WriteAsync(advocate?.Super == true ? ",L2" : ",L1");

						await sw.WriteAsync(advocate?.Status == AdvocateStatus.Blocked ? ",Yes" : ",");
						await sw.WriteAsync("," + advocate?.Brands.Where(x => x.Blocked).Select(b => b.Brand.Name).Concatenate("|").ToString());
						await sw.WriteAsync("," + advocate?.Brands.Where(x => !x.Authorized && !x.Blocked).Select(b => b.Brand.Name).Concatenate("|").ToString());

						// Add the brands columns to the current application
						foreach (var brand in brands)
						{
							advAppDict.TryGetValue((advocate.Id, brand.Id), out var advocateBrand);
							await sw.WriteAsync(
								$",{((advocateBrand?.AuthorizedDate is null) ? (advocateBrand?.Authorized ?? false) ? "Yes" : "" : advocateBrand?.AuthorizedDate.Value.ToShortDateString())}," +
								$" {((advocateBrand?.ContractAcceptedDate is null) ? (advocateBrand?.ContractAccepted ?? false) ? "Yes" : "" : advocateBrand?.ContractAcceptedDate.Value.ToShortDateString())}, " +
								$" {((advocateBrand?.InductedDate is null) ? (advocateBrand?.Inducted ?? false) ? "Yes" : "" : advocateBrand?.InductedDate.Value.ToShortDateString())}");
						}

						foreach (var area in areas)
						{
							answersDict.TryGetValue((advocate.Id, area.Id), out var listOfAnswer);
							await sw.WriteAsync(listOfAnswer != null && listOfAnswer.Answers.Count > 0 ? $",{(string.Join("|", listOfAnswer.Answers))}" : ",");
						}

						foreach (var skill in skillLists)
						{
							skillDict.TryGetValue((advocate.Id, skill.Id), out var listOfSkill);
							await sw.WriteAsync(listOfSkill != null && listOfSkill.Answers != "" ? $",{(listOfSkill.Answers)}" : ",");
						}

						await sw.WriteAsync(Environment.NewLine);
					}
				}

				csv = sw.ToString();
			}

			return csv;
		}

		private async Task<IList<AdminAnswerModel>> GetListOfAnswers(IList<AdvocateApplication> advocateApplications, IList<Area> areas)
		{
			var questions = await _questionRepository.GetAllAsync(include: x => x.Include(y => y.QuestionType));
			var listOfAnswers = new List<AdminAnswerModel>();

			foreach (var advocateApplication in advocateApplications)
			{
				foreach (var area in areas)
				{
					var listOfAnswer = new AdminAnswerModel();

					var applicantionAnswerslist = advocateApplications.Where(x => x.Id == advocateApplication.Id).FirstOrDefault();

					var applicationAnswer = applicantionAnswerslist.ApplicationAnswers.Where(x => x.Question.AreaId == area.Id && x.AdvocateApplicationId == advocateApplication.Id)
																						.OrderByDescending(x => x.Question.Order).FirstOrDefault();
					var answers = applicationAnswer != null ? _applicationAnswerRepository.GetFormattedApplicationQuestionsAnswers(
														applicantionAnswerslist
															.ApplicationAnswers.Where(x =>
																x.QuestionId == applicationAnswer.QuestionId &&
																x.AdvocateApplicationId == applicationAnswer.AdvocateApplicationId).ToList(),
														questions) : new List<string>();
					if (applicationAnswer != null)
					{
						listOfAnswer.AdvocateId = applicationAnswer.AdvocateApplicationId;
						listOfAnswer.AreaId = area.Id;
						listOfAnswer.QuestionId = applicationAnswer.QuestionId;
						listOfAnswer.Answers = answers;
						listOfAnswers.Add(listOfAnswer);
					}
				}
			}

			return listOfAnswers;
		}

		private async Task<List<QuestionOption>> GetListOfSkillsQuestionOptions()
		{
			var questions = await _questionRepository.GetAllAsync(
				predicate: x => x.Area.Title == Area.Skills,
				include: x => x
				.Include(y => y.QuestionType)
				.Include(y => y.Area)
				.Include(y => y.QuestionOptions),
				orderBy: x => x.OrderBy(y => y.Order)
			);

			var skillLists = new List<QuestionOption>();

			skillLists.AddRange(questions.SingleOrDefault(x => x.QuestionType.IsSlider == true && x.QuestionType.IsAllRequired == true).QuestionOptions.OrderBy(x => x.Order));
			skillLists.AddRange(questions.SingleOrDefault(x => x.QuestionType.IsSlider == false && x.QuestionType.IsMultiChoice == true).QuestionOptions.Where(x => x.Text == QuestionOption.ComplexTechnicalSupport));
			skillLists.AddRange(questions.SingleOrDefault(x => x.QuestionType.IsSlider == true && x.QuestionType.IsAllRequired == false).QuestionOptions.OrderBy(x => x.Order));

			return skillLists;
		}

		private IList<AdminSkillAnswerModel> GetListOfSkilsAnswers(IList<AdvocateApplication> advocateApplications, List<QuestionOption> questionOptions)
		{
			var listOfSkillsAnswers = new List<AdminSkillAnswerModel>();
			foreach (var advocateApplication in advocateApplications)
			{
				foreach (var questionOption in questionOptions)
				{
					var listOfSkillAnswer = new AdminSkillAnswerModel();

					var applicantionAnswerslist = advocateApplications
						.Where(x => x.Id == advocateApplication.Id)
						.FirstOrDefault();

					var answerList = applicantionAnswerslist.ApplicationAnswers
						.Where(x => x.QuestionId == questionOption.QuestionId)
						.Select(x => x.Answers).SingleOrDefault();

					if (answerList != null)
					{
						var answers = _applicationAnswerRepository.GetFormattedApplicationSkillAnswers(answerList, questionOption);

						listOfSkillAnswer.Answers = answers;
						listOfSkillAnswer.QuestionOptionId = questionOption.Id;
						listOfSkillAnswer.AdvocateId = advocateApplication.Id;
						listOfSkillsAnswers.Add(listOfSkillAnswer);
					}
				}
			}

			return listOfSkillsAnswers;
		}

		/// <inheritdoc />
		public Task SetBrands(Guid advocateId, IEnumerable<Guid> brandIds, bool authorised) => _mediator.SendCommand(new SetAdvocateBrandsCommand(advocateId, brandIds, authorised));

		/// <inheritdoc/>
		public async Task<Guid?> CreateSuperSolver(string firstName, string lastName, string email, string countryCode, string phone, string password) =>
			await _mediator.SendCommand(new CreateSuperSolverIdentityCommand(Guid.NewGuid(), firstName, lastName, email, countryCode, phone, password));

		/// <inheritdoc/>
		public Task<bool> IsVerified(Guid advocateId) => _advocateRepository.ExistsAsync(adv => adv.Id == advocateId && adv.User.Enabled && adv.VideoWatched && adv.PracticeComplete);
	}
}