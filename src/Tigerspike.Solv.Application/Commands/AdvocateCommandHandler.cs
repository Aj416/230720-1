using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class AdvocateCommandHandler : CommandHandler,
		IRequestHandler<CreateAdvocateCommand, Unit>,
		IRequestHandler<CreateAdvocateIdentityCommand, Unit>,
		IRequestHandler<SetVideoWatchedCommand, Unit>,
		IRequestHandler<SetIdentityApplicantCommand, Unit>,
		IRequestHandler<SetIdentityCheckDetailsCommand, Unit>,
		IRequestHandler<SetIdentityVerificationStatusCommand, Unit>,
		IRequestHandler<UpdateAdvocatePaymentAccountCommand, Unit>,
		IRequestHandler<StartAdvocatePracticeCommand, Unit>,
		IRequestHandler<FinishAdvocatePracticeCommand, Unit>,
		IRequestHandler<AcceptBrandAgreementCommand, Unit>,
		IRequestHandler<AgreeToContractCommand, Unit>,
		IRequestHandler<CompleteInductionCommand, Unit>,
		IRequestHandler<EnableBrandCommand, Unit>,
		IRequestHandler<DisableBrandCommand, Unit>,
		IRequestHandler<AssignBrandsToNewAdvocateCommand, Unit>,
		IRequestHandler<SetAdvocateBrandsCommand, Unit>,
		IRequestHandler<UpdateAdvocateCsatCommand, Unit>,
		IRequestHandler<MarkInductionItemCommand, Unit>,
		IRequestHandler<SetGuideLineCommand, Unit>,
		IRequestHandler<AttemptAdvocateQuizCommand, bool>,
		IRequestHandler<UpdateAdvocateProfileStatusCommand, Unit>
	{
		private readonly IPaymentService _paymentService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IAdvocateBrandRepository _advocateBrandRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IQuizAdvocateAttemptRepository _quizAdvocateAttemptRepository;
		private readonly IQuestionRepository _questionRepository;
		private readonly IUserRepository _userRepository;
		private readonly ITimestampService _timestampService;
		private readonly ILogger<AdvocateCommandHandler> _logger;
		private readonly IFeatureManager _featureManager;

		public AdvocateCommandHandler(
			IAdvocateRepository advocateRepository,
			IAdvocateBrandRepository advocateBrandRepository,
			ITicketRepository ticketRepository,
			IBrandRepository brandRepository,
			IAdvocateApplicationRepository advocateApplicationRepository,
			IQuizAdvocateAttemptRepository quizAdvocateAttemptRepository,
			IQuestionRepository questionRepository,
			IUserRepository userRepository,
			IPaymentService paymentService,
			IAuthenticationService authenticationService,
			IWebHostEnvironment hostingEnvironment,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			ITimestampService timestampService,
			IDomainNotificationHandler notifications,
			ILogger<AdvocateCommandHandler> logger,
			IFeatureManager featureManager) : base(uow, mediator, notifications)
		{
			_hostingEnvironment = hostingEnvironment ??
				throw new ArgumentNullException(nameof(hostingEnvironment));
			_featureManager = featureManager ??
				throw new ArgumentNullException(nameof(featureManager));
			_advocateRepository = advocateRepository ??
				throw new ArgumentNullException(nameof(advocateRepository));
			_advocateBrandRepository = advocateBrandRepository ??
				throw new ArgumentNullException(nameof(advocateBrandRepository));
			_ticketRepository = ticketRepository ??
				throw new ArgumentNullException(nameof(ticketRepository));
			_advocateApplicationRepository = advocateApplicationRepository ??
				throw new ArgumentNullException(nameof(advocateApplicationRepository));
			_brandRepository = brandRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_quizAdvocateAttemptRepository = quizAdvocateAttemptRepository ??
				throw new ArgumentNullException(nameof(quizAdvocateAttemptRepository));
			_questionRepository = questionRepository ??
				throw new ArgumentNullException(nameof(questionRepository));
			_userRepository = userRepository ?? 
				throw new ArgumentNullException(nameof(userRepository));
			_paymentService = paymentService ??
				throw new ArgumentNullException(nameof(paymentService));
			_authenticationService = authenticationService ??
				throw new ArgumentNullException(nameof(authenticationService));
			_timestampService = timestampService ??
					throw new ArgumentNullException(nameof(timestampService));
			_logger = logger;
		}

		public async Task<Unit> Handle(CreateAdvocateIdentityCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			if (advocate != null)
			{
				_logger.LogError("CreateAdvocateIdentityCommand - Duplicate advocate {0}", request.AdvocateId);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate already exists"));
				return Unit.Value;
			}

			var user = await _userRepository.GetByEmail(request.Email.Trim());
			if (user != null)
			{
				_logger.LogError("CreateAdvocateIdentityCommand - Create advocate {0} failed as duplicate user {1} exists with id {2}", request.AdvocateId, request.Email, user.Id);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The email already exists"));
				return Unit.Value;
			}

			try
			{
				await _authenticationService.CreateAdvocate(request.AdvocateId, request.FirstName, request.LastName, request.Email, request.Password);
			}
			catch (ServiceInvalidOperationException ex)
			{
				_logger.LogError(ex, "Auth0 Error on CreateAdvocateIdentityCommand handler");
				string[] envs = { "local", "docker" };

				// TODO: I'm not satisified about this code and will never accept it, however, it was requested temporarily to help FE people test quickly.
				// PS: Think what happens if Auth0 changes the message! Only excecute this if it is
				// not local/docker or the message is not about existing user.
				if (!(envs.Contains(_hostingEnvironment.EnvironmentName.ToLower()) && ex.Message == "The user already exists."))
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, ex.Message));
					return Unit.Value;
				}
			}

			await _mediator.RaiseEvent(new AdvocateIdentityCreatedEvent(request.AdvocateId, request.FirstName,
				request.LastName, request.Email, request.Phone, request.CountryCode, request.Source,
				request.InternalAgent, SolvRoles.Advocate));

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateAdvocateCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);

			if (advocate != null)
			{
				_logger.LogError("CreateAdvocateCommand - Advocate {0} already exists", request.AdvocateId);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate already exists"));
				return Unit.Value;
			}

			var user = new User(request.AdvocateId, request.FirstName, request.LastName, request.Email, request.Phone);

			advocate = new Advocate(user, request.CountryCode, request.Source, request.InternalAgent, request.Verified, request.SuperSolver);
			await _advocateRepository.InsertAsync(advocate, cancellationToken);

			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateId);
			if (advocateApplication != null)
			{
				advocateApplication.Finish();
				_advocateApplicationRepository.Update(advocateApplication);
			}
			else
			{
				advocateApplication = new AdvocateApplication(request.FirstName, request.LastName, request.Email, request.Phone, null, request.CountryCode, request.Source, request.InternalAgent, null, null, null, id: request.AdvocateId);
				_advocateApplicationRepository.Insert(advocateApplication);
			}

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateCreatedEvent(advocate.Id));
				return Unit.Value;
			}
			
			_logger.LogError("Commmit failed on CreateAdvocateCommand handler");
			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The account creation could not be completed due to system error. Please contact administrator."));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetVideoWatchedCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);

			if (advocate == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate doesn't exists"));
				return Unit.Value;
			}

			advocate.SetVideoWatched();

			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateVideoWatchedEvent(request.AdvocateId));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateAdvocatePaymentAccountCommand request, CancellationToken cancellationToken)
		{
			(var merchantId, var paymentReceivable, var emailVerified) = await _paymentService.GetPayPalSolverStatus(request.AdvocateId);

			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);

			// if the permissions were granted by PayPal we save the account id, otherwise we clear it.
			advocate.SetPaymentAccount(paymentReceivable ? merchantId : (string)null, emailVerified);

			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocatePaymentAccountUpdatedEvent(request.AdvocateId));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(StartAdvocatePracticeCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.Queryable().Where(x => x.Id == request.AdvocateId)
				.Include(x => x.Brands).FirstOrDefaultAsync();

			advocate.StartPractise(_featureManager.IsEnabledAsync(nameof(FeatureFlags.SetupProfile)).Result);

			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				// raise event AdvocatePractiseStartedEvent
				await _mediator.RaiseEvent(new AdvocatePractiseStartedEvent(advocate.Id));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(FinishAdvocatePracticeCommand request, CancellationToken cancellationToken)
		{
			var advocateId = await _ticketRepository.GetSingleOrDefaultAsync(selector: t => t.AdvocateId, predicate: t => t.Id == request.TicketId);
			var advocate = await _advocateRepository.GetFullAdvocateAsync(a => a.Id == advocateId.Value);
			advocate.FinishPractice();

			// TODO: Assign another brand to him when we have this functionality.
			// var brand = await _brandRepository.GetNewBrandForUser(advocate.UserId);
			// advocate.AddBrand(brand.BrandId);

			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocatePractiseFinishedEvent(advocate.Id));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Couldn't finish practice mode"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(AcceptBrandAgreementCommand request, CancellationToken cancellationToken)
		{
			var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, request.BrandId);
			if (advocateBrand != null)
			{
				advocateBrand.AcceptBrandAgreement();
				_advocateBrandRepository.Update(advocateBrand);

				var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
				advocate.SetBrandNotification(advocateBrand);

				if (await Commit())
				{
					// if necessary in future, raise event that a contract has been agreed
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Advocate/Brand cannot be found.", (int)(HttpStatusCode.NotFound)));
			}


			return Unit.Value;
		}

		public async Task<Unit> Handle(AgreeToContractCommand request, CancellationToken cancellationToken)
		{
			var isAutomaticAuthorization = await _brandRepository.GetSingleOrDefaultAsync(selector: s => s.AutomaticAuthorization, predicate: p => p.Id == request.BrandId);
			var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, request.BrandId);
			if (advocateBrand != null)
			{
				advocateBrand.AgreeToContract(isAutomaticAuthorization, _timestampService.GetUtcTimestamp());
				_advocateBrandRepository.Update(advocateBrand);

				var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
				advocate.SetBrandNotification(advocateBrand);

				if (await Commit())
				{
					await _mediator.RaiseEvent(new AdvocateContractAgreedEvent(request.AdvocateId, request.BrandId));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Advocate/Brand cannot be found.", (int)(HttpStatusCode.NotFound)));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CompleteInductionCommand request, CancellationToken cancellationToken)
		{
			var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, request.BrandId);
			if (advocateBrand != null)
			{
				var isAutomaticAuthorization = await _brandRepository.GetSingleOrDefaultAsync(selector: s => s.AutomaticAuthorization, predicate: p => p.Id == request.BrandId);
				advocateBrand.PassInduction(isAutomaticAuthorization, _timestampService.GetUtcTimestamp());
				_advocateBrandRepository.Update(advocateBrand);

				var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
				advocate.SetBrandNotification(advocateBrand);

				if (await Commit())
				{
					await _mediator.RaiseEvent(new AdvocateInductionCompletedEvent(request.AdvocateId, request.BrandId));
					if (advocateBrand.Enabled)
					{
						await _mediator.RaiseEvent(new AdvocateBrandEnabledEvent(request.AdvocateId, request.BrandId));
					}

					return Unit.Value;
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Advocate/Brand cannot be found.", (int)(HttpStatusCode.NotFound)));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(EnableBrandCommand request, CancellationToken cancellationToken)
		{
			await HandleEnableDisableBrandCommand(request, request.AdvocateId, request.BrandId, true);
			return Unit.Value;
		}

		public async Task<Unit> Handle(DisableBrandCommand request, CancellationToken cancellationToken)
		{
			await HandleEnableDisableBrandCommand(request, request.AdvocateId, request.BrandId, false);
			return Unit.Value;
		}

		public async Task<Unit> Handle(AssignBrandsToNewAdvocateCommand request, CancellationToken cancellationToken)
		{
			var brandIds = await _brandRepository.GetBrandsForAdvocateApplication(request.AdvocateId);

			if (!brandIds.Any())
			{
				_logger.LogError("AssignBrandsToNewAdvocateCommand - BrandIds not found for advocate request {0}", request.AdvocateId);
				return Unit.Value;
			}

			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			advocate.SetBrands(brandIds, false);
			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new NewAdvocateBrandsAssignedEvent(advocate.Id));
				return Unit.Value;
			}

			_logger.LogError("AssignBrandsToNewAdvocateCommand - Commit failed on assign brands for advocate {0}", request.AdvocateId);
			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Couldn't assign brands"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetAdvocateBrandsCommand request, CancellationToken cancellationToken)
		{
			var brandIdsCount = await _brandRepository.CountAsync(b => request.BrandIds.Contains(b.Id));

			if (brandIdsCount != request.BrandIds.Count())
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Some brand Ids don't exist"));
				return Unit.Value;
			}

			var advocate = await _advocateRepository.GetFullAdvocateAsync(predicate: a => a.Id == request.AdvocateId);

			if (advocate == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Advocate doesn't exist"));
				return Unit.Value;
			}

			var (blockedBrands, addedBrands, unblockedBrands) = advocate.SetBrands(request.BrandIds, request.Authorised);
			advocate.SetBlockedHistory(blockedBrands, _timestampService.GetUtcTimestamp());
			_advocateRepository.Update(advocate);

			if (addedBrands.Count > 0 || unblockedBrands.Count > 0)
			{
				var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateId);
				advocateApplication.LastInvitationDate = _timestampService.GetUtcTimestamp();
				_advocateApplicationRepository.Update();
			}

			foreach (var brandId in blockedBrands)
			{
				var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, brandId);
				advocateBrand.SetBlocked(true);
				_advocateBrandRepository.Update(advocateBrand);
			}

			foreach (var brandId in unblockedBrands)
			{
				var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, brandId);
				advocateBrand.SetBlocked(false);
				_advocateBrandRepository.Update(advocateBrand);
			}

			if (await Commit())
			{
				if (addedBrands.Count > 0 || unblockedBrands.Count > 0)
				{
					await _mediator.RaiseEvent(new AdvocateBrandsAssignedEvent(advocate.Id, advocate.User.FirstName, advocate.User.Email));
				}

				if (blockedBrands.Count > 0)
				{
					await _mediator.RaiseEvent(new AdvocateBrandsRemovedEvent(advocate.Id, advocate.User.Email, advocate.User.FirstName, blockedBrands));
				}

				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Couldn't assign brands"));
			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateAdvocateCsatCommand request, CancellationToken cancellationToken)
		{
			var allBrandsCsat = (decimal)await _ticketRepository.GetAverageCsat(advocateId: request.AdvocateId);
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			advocate.SetCsat(allBrandsCsat);

			var brandCsat = (decimal)await _ticketRepository.GetAverageCsat(advocateId: request.AdvocateId, brandId: request.BrandId);
			var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, request.BrandId);
			advocateBrand.SetCsat(brandCsat);

			_advocateRepository.Update(advocate);
			_advocateBrandRepository.Update(advocateBrand);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateCsatUpdatedEvent(advocateBrand.AdvocateId, advocateBrand.BrandId));
				return Unit.Value;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Couldn't update advocate csat"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(MarkInductionItemCommand request, CancellationToken cancellationToken)
		{
			var sectionItem = await _brandRepository.GetInductionSectionItem(request.ItemId);

			if (sectionItem == null || !sectionItem.Enabled)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"Couldn't set induction section item to viewed"));

				return Unit.Value;
			}

			var advocate = await _advocateRepository.GetSingleOrDefaultAsync(a => a.Id == request.AdvocateId, null,
				a => a.Include(si => si.AdvocateSectionItems), false);

			if (advocate.AdvocateSectionItems.All(s => s.SectionItemId != request.ItemId))
			{
				advocate.AdvocateSectionItems.Add(new AdvocateSectionItem(request.AdvocateId, request.ItemId));

				_advocateRepository.Update(advocate);

				if (await Commit())
				{
					await _mediator.RaiseEvent(new InductionItemViewedEvent(request.AdvocateId, request.BrandId, request.ItemId));

					return Unit.Value;
				}
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Couldn't set induction section item to viewed"));

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetGuideLineCommand request, CancellationToken cancellationToken)
		{
			var advocateBrand = await _advocateBrandRepository.FindAsync(request.AdvocateId, request.BrandId);
			if (advocateBrand != null)
			{
				advocateBrand.SetGuideline(true);
				_advocateBrandRepository.Update(advocateBrand);

				await Commit();
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Advocate/Brand cannot be found.", (int)(HttpStatusCode.NotFound)));
			}
			return Unit.Value;
		}

		public async Task<Unit> Handle(SetIdentityApplicantCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			advocate.SetIdentityApplicant(request.IdentityApplicantId);
			_advocateRepository.Update(advocate);

			await Commit();


			return Unit.Value;
		}

		public async Task<Unit> Handle(SetIdentityCheckDetailsCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			advocate.SetIdentityCheckDetails(request.IdentityCheckId, request.IdentityCheckResultUrl);
			advocate.SetIdentityVerificationStatus(IdentityVerificationStatus.Processing);
			_advocateRepository.Update(advocate);

			await Commit();

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetIdentityVerificationStatusCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);
			advocate.SetIdentityVerificationStatus(request.IdentityVerificationStatus);
			_advocateRepository.Update(advocate);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateIdentityVerificationStatusUpdatedEvent(request.AdvocateId));
			}

			return Unit.Value;
		}

		private async Task HandleEnableDisableBrandCommand<T>(Command<T> request, Guid advocateId, Guid brandId, bool value)
		{
			var advocateBrand = await _advocateBrandRepository.FindAsync(advocateId, brandId);
			if (advocateBrand != null)
			{
				advocateBrand.SetEnabled(value);
				_advocateBrandRepository.Update(advocateBrand);

				if (await Commit())
				{
					if (advocateBrand.Enabled)
					{
						await _mediator.RaiseEvent(new AdvocateBrandEnabledEvent(advocateId, brandId));
					}
					else
					{
						await _mediator.RaiseEvent(new AdvocateBrandDisabledEvent(advocateId, brandId));
					}

				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Advocate/Brand cannot be found.", (int)(HttpStatusCode.NotFound)));
			}
		}

		public async Task<bool> Handle(AttemptAdvocateQuizCommand request, CancellationToken cancellationToken)
		{
			var advocateAttempt = new QuizAdvocateAttempt(request.AdvocateId, request.QuizId,
			request.Result);

			await _quizAdvocateAttemptRepository.InsertAsync(advocateAttempt);

			advocateAttempt.AddAnswers(request.Answers);

			if (await Commit())
			{
				// Raise any event required in future.
				_quizAdvocateAttemptRepository.Update(advocateAttempt);
				await _mediator.RaiseEvent(new AdvocateQuizAttemptedEvent(request.AdvocateId));
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"Failed to save {request.AdvocateId} advocate's attempt for quiz {request.QuizId}."));
			return false;
		}

		public async Task<Unit> Handle(UpdateAdvocateProfileStatusCommand request, CancellationToken cancellationToken)
		{
			var advocate = await _advocateRepository.FindAsync(request.AdvocateId);

			var question = await _questionRepository.FindAsync(request.ProfileQuestionId);

			var lastQuestionOrder = await _questionRepository.GetFirstOrDefaultAsync(
					predicate: p => p.AreaId == question.AreaId && p.Enabled,
					selector: s => s.Order,
					orderBy: ob => ob.OrderByDescending(q => q.Order)
				);

			if (advocate == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate doesn't exists"));
				return Unit.Value;
			}

			if (advocate.ProfileStatus == AdvocateApplicationProfileStatus.NotStarted)
			{
				advocate.ProfileStatus = AdvocateApplicationProfileStatus.Started;

				_advocateRepository.Update(advocate);

				if (await Commit())
				{
					// Raise any event if required.
					return Unit.Value;
				}

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Failed to save profile status for advocate"));

			}

			if (question.Order == lastQuestionOrder)
			{
				advocate.ProfileStatus = AdvocateApplicationProfileStatus.Completed;

				_advocateRepository.Update(advocate);

				if (await Commit())
				{
					// Raise any event if required.
					return Unit.Value;
				}

				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Failed to save profile status for advocate"));
			}

			return Unit.Value;
		}
	}
}