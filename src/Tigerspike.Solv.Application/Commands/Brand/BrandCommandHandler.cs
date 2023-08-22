using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers.Brand
{
	public class BrandCommandHandler : CommandHandler,
		IRequestHandler<UpdateBrandPaymentAccountCommand, Unit>,
		IRequestHandler<UpdateBillingAgreementTokenCommand, Unit>,
		IRequestHandler<AssignBrandsToAdvocateApplicationsCommand, Unit>,
		IRequestHandler<CreateBrandCommand, Guid?>,
		IRequestHandler<CreateApiKeyCommand, Unit>,
		IRequestHandler<CreateQuizCommand, Unit>,
		IRequestHandler<CreateAbandonReasonCommand, Unit>,
		IRequestHandler<CreateTagCommand, Unit>,
		IRequestHandler<SetBrandContractCommand, Unit>,
		IRequestHandler<CreateCategoryCommand, Unit>,
		IRequestHandler<SetBrandBillingDetailsIdCommand, Unit>
	{
		private readonly IPaymentService _paymentService;
		private readonly IApiKeyRepository _apiKeyRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IAdvocateApplicationBrandRepository _advocateApplicationBrandRepository;
		private readonly IMapper _mapper;
		private readonly IQuizRepository _quizRepository;

		public BrandCommandHandler(
			IPaymentService paymentService,
			IApiKeyRepository apiKeyRepository,
			IBrandRepository brandRepository,
			IAdvocateApplicationRepository advocateApplicationRepository,
			IAdvocateApplicationBrandRepository advocateApplicationBrandRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IMapper mapper,
			IDomainNotificationHandler notifications,
			IQuizRepository quizRepository) : base(uow, mediator, notifications)
		{
			_paymentService = paymentService ??
				throw new ArgumentNullException(nameof(paymentService));
			_apiKeyRepository = apiKeyRepository;
			_brandRepository = brandRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_advocateApplicationRepository = advocateApplicationRepository ??
				throw new ArgumentNullException(nameof(brandRepository));
			_advocateApplicationBrandRepository = advocateApplicationBrandRepository;
			_mapper = mapper;
			_quizRepository = quizRepository ??
				throw new ArgumentNullException(nameof(quizRepository));
		}

		public async Task<Unit> Handle(UpdateBrandPaymentAccountCommand request, CancellationToken cancellationToken)
		{
			var storedToken = await _brandRepository.GetFirstOrDefaultAsync(x => x.BillingAgreementToken, x => x.Id == request.BrandId);
			if (storedToken != request.BillingAgreementToken)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "Billing agreement token is incorrect"));
				return Unit.Value;
			}

			var (payerId, billingAgreementId) = await _paymentService.FinalizeBillingAgreement(request.BrandId, request.BillingAgreementToken);

			var brand = await _brandRepository.FindAsync(request.BrandId);

			brand.SetPaymentAccount(payerId, billingAgreementId);

			_brandRepository.Update(brand);

			if (await Commit())
			{
				// raise event
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateBillingAgreementTokenCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.FindAsync(request.BrandId);
			brand.SetBillingAgreementToken(request.BillingAgreementToken);
			_brandRepository.Update(brand);

			if (await Commit())
			{
				// raise event
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(AssignBrandsToAdvocateApplicationsCommand request, CancellationToken cancellationToken)
		{
			var areApplicationsValid = await _advocateApplicationRepository.AreApplicationsNotInvited(request.AdvocateApplicationIds);
			if (!areApplicationsValid)
			{
				await _mediator.RaiseEvent(new DomainNotification(CommitErrorKey, "One of the advocate application ids cannot be found or already invited.", (int)HttpStatusCode.NotFound));
				return Unit.Value;
			}

			var areBrandsValid = await _brandRepository.AreAssignable(request.BrandIds);
			if (!areBrandsValid)
			{
				await _mediator.RaiseEvent(new DomainNotification(CommitErrorKey, "One of the brands cannot be found or is a practice brand.", (int)HttpStatusCode.NotFound));
				return Unit.Value;
			}

			var assignments =
				from advocateApplicationId in request.AdvocateApplicationIds
				from brandId in request.BrandIds
				select new AdvocateApplicationBrand(advocateApplicationId, brandId);

			await _advocateApplicationBrandRepository.InsertAsync(assignments);

			if (await Commit())
			{
				// raise event
			}

			return Unit.Value;
		}

		public async Task<Guid?> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
		{
			var brand = _mapper.Map<Domain.Models.Brand>(request);
			await _brandRepository.InsertAsync(brand);
			brand.SetForcedEscalationReason(AbandonReason.ForcedEscalationReasonName);
			brand.SetBlockedAdvocateReason(AbandonReason.BlockedAdvocateReasonName);
			brand.SetAutoAbandonedReason(AbandonReason.AutoAbandonedReasonName);

			if (await Commit())
			{
				return brand.Id;
			}
			else
			{
				return null;
			}
		}
		public async Task<Unit> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
		{
			if (await _apiKeyRepository.GetBrandIdFromApiKey(request.M2M) == null)
			{
				if (await _apiKeyRepository.GetBrandIdFromApplicationId(request.SDK) == null)
				{
					var apiKey = new ApiKey(request.BrandId, request.M2M, request.SDK);
					await _apiKeyRepository.InsertAsync(apiKey);

					if (await Commit())
					{
						// raise event if you like
					}
					else
					{
						await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new api key for {request.BrandId}"));
					}
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"SDK key has to be unique ({request.SDK})"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"M2M ApiKey has to be unique ({request.M2M})"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == request.BrandId
			);

			if (brand != null)
			{
				var questions = request.Questions.Select(x =>
				new QuizQuestion(x.name, x.isMultiChoice, request.Questions.IndexOf(x), x.options.Select(y =>
				new QuizQuestionOption(y.text, y.correct, x.options.IndexOf(y))).ToList()));

				var quiz = new Quiz(request.Title, request.Description, request.FailureMessage, request.SuccessMessage, request.AllowedMistakes, questions.ToList());

				await _quizRepository.InsertAsync(quiz);

				brand.AssignQuizId(quiz.Id);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new quiz for brand {request.BrandId} {brand.Name} / {request.Title}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateAbandonReasonCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(i => i.AbandonReasons),
				disableTracking: false
			);

			if (brand != null)
			{
				brand.AddAbandonReason(request.Name, request.Action);

				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to add abandon reasons for brand {request.BrandId} {brand.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateTagCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(i => i.Tags),
				disableTracking: false
			);

			if (brand != null)
			{
				var brandTag = _mapper.Map<Tag>(request);

				brand.Tags.Add(brandTag);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to add tags for brand {request.BrandId} {brand.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetBrandContractCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == request.BrandId,
				disableTracking: false
			);

			if (brand != null)
			{
				brand.SetContract(request.ContractUrl);

				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update contract for a brand {request.BrandId} {brand.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(i => i.Categories),
				disableTracking: false
			);

			if (brand != null)
			{
				if (!brand.CategoryEnabled)
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Enable Category flag for brand {request.BrandId} {brand.Name} before proceeding"));
					return Unit.Value;
				}

				brand.AddCategories(request.Categories);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to add categories for brand {request.BrandId} {brand.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Brand {request.BrandId} is not found"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(SetBrandBillingDetailsIdCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.FindAsync(request.BrandId);

			if (brand != null)
			{
				brand.AssignBillingDetailsId(request.BillingDetailsId);

				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update billing details id for brand {request.BrandId}"));
				}
			}

			return Unit.Value;
		}
	}
}