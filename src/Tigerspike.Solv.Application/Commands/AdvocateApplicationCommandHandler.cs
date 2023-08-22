using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Application.Commands;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.CommandHandlers
{
	public class AdvocateApplicationCommandHandler : CommandHandler,
		IRequestHandler<CreateAdvocateApplicationCommand, Guid?>,
		IRequestHandler<InviteAdvocateCommand, bool>,
		IRequestHandler<DeclineAdvocateCommand, bool>,
		IRequestHandler<DeleteAdvocateApplicationCommand, bool>,
		IRequestHandler<ExportAdvocateApplicationCommand, bool>,
		IRequestHandler<SendDeleteAdvocateApplicationCommand, bool>,
		IRequestHandler<SetAdvocateApplicationResponseEmailSentCommand, Unit>,
		IRequestHandler<SetAdvocateApplicationCompletedEmailSentCommand, Unit>,
		IRequestHandler<SubmitAdvocateApplicationAnswersCommand, bool>,
		IRequestHandler<ChangeAdvocateApplicationNameCommand, Unit>,
		IRequestHandler<UpdateSuperSolverCommand, Unit>,
		IRequestHandler<SubmitAdvocateApplicationProfileCommand, bool>
	{
		private readonly IAdvocateApplicationRepository _advocateApplicationRepository;
		private readonly IApplicationAnswerRepository _applicationAnswerRepository;
		private readonly EmailTemplatesOptions _emailTemplateOptions;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILogger<AdvocateApplicationCommandHandler> _logger;
		private readonly IAdvocateService _advocateService;
		private readonly INewProfileService _newProfileService;

		public AdvocateApplicationCommandHandler(
			IAdvocateService advocateService,
			IAdvocateApplicationRepository advocateApplicationRepository,
			IApplicationAnswerRepository applicationAnswerRepository,
			IOptions<EmailTemplatesOptions> emailTemplateOptions,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IBus bus,
			IOptions<BusOptions> busOptions,
			ILogger<AdvocateApplicationCommandHandler> logger,
			IDomainNotificationHandler notifications,
			INewProfileService newProfileService) : base(uow, mediator, notifications)
		{
			_newProfileService = newProfileService ??
				throw new ArgumentNullException(nameof(newProfileService));
			_advocateService = advocateService ??
				throw new ArgumentNullException(nameof(advocateService));
			_advocateApplicationRepository = advocateApplicationRepository ??
				throw new ArgumentNullException(nameof(advocateApplicationRepository));
			_applicationAnswerRepository = applicationAnswerRepository ??
				throw new ArgumentNullException(nameof(applicationAnswerRepository));
			_emailTemplateOptions = emailTemplateOptions?.Value ??
				throw new ArgumentNullException(nameof(emailTemplateOptions));
			_bus = bus ??
				   throw new ArgumentNullException(nameof(bus));
			_busOptions = busOptions.Value;
			_logger = logger;
		}

		public async Task<Guid?> Handle(CreateAdvocateApplicationCommand request, CancellationToken cancellationToken)
		{

			var application = new AdvocateApplication(request.FirstName, request.LastName, request.Email, request.Phone,
				request.State, request.Country, request.Source, request.InternalAgent, request.Address, request.City, request.ZipCode);

			await _advocateApplicationRepository.InsertAsync(application);

			if (await Commit())
			{
				if (_newProfileService.NewProfileEnable())
				{
					//if new profile feature enable then run this code block
					await _advocateService.Create(application.Token, request.Password);
				}

				await _mediator.RaiseEvent(new AdvocateApplicationCreatedEvent(application.Id,
					application.FirstName, application.LastName, application.Email, application.InternalAgent));

				return application.Id;
			}

			_logger.LogError("CreateAdvocateApplicationCommand - Commit failed to create advocate application");
			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Advocate Application cannot be created"));

			return null;
		}

		public async Task<bool> Handle(InviteAdvocateCommand request, CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateApplicationId);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"The advocate application ID cannot be found."));
				return false;
			}

			// Don't send an invite if the applicant has already been invited, or already has an account
			if (advocateApplication.CompletedEmailSent == false ||
				advocateApplication.ApplicationStatus == AdvocateApplicationStatus.AccountCreated)
			{
				return false;
			}

			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));
			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = advocateApplication.Email,
				Subject = _emailTemplateOptions.AdvocateAcceptedEmailSubject,
				Template = EmailTemplates.AdvocateAccepted.ToString(),
				Model = new Dictionary<string, object>
					{
						{"AdvocateAcceptedEmailSubject", _emailTemplateOptions.AdvocateAcceptedEmailSubject},
						{"EmailLogoLocation", _emailTemplateOptions.EmailLogoLocation},
						{"AdvocateSignUpUrl", _emailTemplateOptions.AdvocateSignUpUrl},
						{"Code", advocateApplication.Token},
						{"AdvocateAcceptedEmailIllustrationLocation", _emailTemplateOptions.AdvocateAcceptedEmailIllustrationLocation},
						{"MarketingSiteAuthenticatorAppUrl", _emailTemplateOptions.MarketingSiteAuthenticatorAppUrl}
					}
			}, cancellationToken);

			advocateApplication.Invite();

			_advocateApplicationRepository.Update(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationInvitedEvent(request.AdvocateApplicationId));
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Could not commit the InviteAdvocateCommand."));

			return false;
		}

		public async Task<bool> Handle(ExportAdvocateApplicationCommand request, CancellationToken cancellationToken)
		{
			// Send the email
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = request.Email,
				Subject = _emailTemplateOptions.AdvocateExportEmailSubject,
				Template = EmailTemplates.ExportAdvocateApplication.ToString(),
				Model = new Dictionary<string, object>
					{
						{"AdvocateExportEmailSubject", _emailTemplateOptions.AdvocateExportEmailSubject},
						{"EmailLogoLocation", _emailTemplateOptions.EmailLogoLocation}
					},
				SenderName = null,
				Attachment = new EmailAttachment
				{
					ContentType = _emailTemplateOptions.AdvocateExportEmailAttachmentContentType,
					Content = request.ApplicationJson,
					Filename = _emailTemplateOptions.AdvocateExportEmailAttachmentFileName,
				}
			}, cancellationToken);

			return true;
		}

		public async Task<bool> Handle(SendDeleteAdvocateApplicationCommand request,
			CancellationToken cancellationToken)
		{
			// Get AdvocateApplication with the supplied email
			var advocateApplication = await _advocateApplicationRepository.GetFirstOrDefaultAsync(predicate: a => a.Email == request.Email);

			if (advocateApplication == null)
			{
				// Although we couldn't find any applications we should not throw an exception
				// TODO: But still, do we really return true if no application is found !??
				return true;
			}

			if (advocateApplication.ApplicationStatus == AdvocateApplicationStatus.AccountCreated)
			{
				return true;
			}

			// Generate a cryptographically secure random SHA-256 hash which will be used by the
			// advocate applicant as a secret key in the case where they wish to confirm the
			// deletion.
			var hashString = string.Empty;
			try
			{
				using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
				{
					var randomData = new byte[256];
					rng.GetBytes(randomData);
					using (var sha256 = SHA256.Create())
					{
						var randomHash = sha256.ComputeHash(randomData);
						hashString = randomHash.Aggregate(hashString,
							(current, singleByte) => current + singleByte.ToString("x2"));
					}
				}
			}
			catch (Exception)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"There was an error generating a deletion hash for the advocate application."));
				return false;
			}

			// Set all application with the supplied email address to this hash string. This means
			// that only the latest email in their inbox can be used to delete their data
			advocateApplication.DeletionHash = hashString;
			_advocateApplicationRepository.Update(advocateApplication);

			if (!await Commit())
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"There was an error updating the deletion hash for the advocate application."));
				return false;
			}

			// Generate the button URL
			var buttonUrl = _emailTemplateOptions.AdvocateDeleteUrl +
				"?" + _emailTemplateOptions.AdvocateDeleteUrlQueryParamEmail +
				"=" + HttpUtility.UrlEncode(request.Email) +
				"&" + _emailTemplateOptions.AdvocateDeleteUrlQueryParamKey +
				"=" + hashString;

			// Send the email with the secret key to the advocate applicant
			var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Notification}"));

			await endpoint.Send<ISendEmailMessageCommand>(new SendEmailMessageCommand
			{
				MailTo = request.Email,
				Subject = _emailTemplateOptions.AdvocateDeleteEmailSubject,
				Template = EmailTemplates.DeleteAdvocateApplication.ToString(),
				Model = new Dictionary<string, object>
					{
						{"AdvocateDeleteEmailSubject", _emailTemplateOptions.AdvocateDeleteEmailSubject},
						{"EmailLogoLocation", _emailTemplateOptions.EmailLogoLocation},
						{"ButtonUrl", buttonUrl}
					}
			}, cancellationToken);

			return true;
		}

		public async Task<bool> Handle(DeleteAdvocateApplicationCommand request, CancellationToken cancellationToken)
		{
			// Get all AdvocateApplications with the supplied email and deletion hash
			var advocateApplication = await
			_advocateApplicationRepository.GetFirstOrDefaultAsync(predicate: x =>
				x.Email == request.Email && x.DeletionHash == request.Key);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"An advocate application with the supplied email address and key cannot be found."));

				return false;
			}

			if (advocateApplication.ApplicationStatus == AdvocateApplicationStatus.AccountCreated)
			{
				return true;
			}

			_advocateApplicationRepository.Delete(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationDeletedEvent(advocateApplication.Id));
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"There was an error deleting the advocates application."));
			return false;
		}

		public async Task<Unit> Handle(SetAdvocateApplicationResponseEmailSentCommand request,
			CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.ApplicationId);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"The advocate application ID cannot be found."));
				return Unit.Value;
			}

			advocateApplication.SetResponseEmailSent();

			_advocateApplicationRepository.Update(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationResponseEmailSentEvent(request.ApplicationId));
			}

			return Unit.Value;
		}

		public async Task<bool> Handle(SubmitAdvocateApplicationAnswersCommand request,
			CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateApplicationId);

			if (advocateApplication == null)
			{
				_logger.LogError("SubmitAdvocateApplicationAnswers - AdvocateApplication {0} doesn't exist", request.AdvocateApplicationId);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate application does not exists"));
				return false;
			}

			var hasAnswers = await _applicationAnswerRepository.HasAnswers(request.AdvocateApplicationId);
			if (hasAnswers)
			{
				_logger.LogError("SubmitAdvocateApplicationAnswers - AdvocateApplication {0} already completed", request.AdvocateApplicationId);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, "The advocate application has already been completed"));
				return false;
			}

			advocateApplication.SetAnswers(request.ApplicationAnswers);

			await _applicationAnswerRepository.InsertAsync(request.ApplicationAnswers, cancellationToken);

			if (await Commit())
			{
				// Raise event
				await _mediator.RaiseEvent(new AdvocateApplicationCompletedEvent(request.AdvocateApplicationId,
					advocateApplication.Email));

				return true;
			}

			_logger.LogError("SubmitAdvocateApplication - Commit failed for AdvocateApplication {0}", request.AdvocateApplicationId);
			return false;
		}

		public async Task<Unit> Handle(SetAdvocateApplicationCompletedEmailSentCommand request,
			CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.ApplicationId);

			if (advocateApplication == null)
			{
				_logger.LogError("SetAdvocateApplicationCompletedEmailSent - AdvocateApplication {0} doesn't exist", request.ApplicationId);
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					"The advocate application ID cannot be found."));
				return Unit.Value;
			}

			advocateApplication.SetCompletedEmailSent();

			_advocateApplicationRepository.Update(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationCompletedEmailSentEvent(request.ApplicationId));
			}

			_logger.LogError("SetAdvocateApplicationCompletedEmailSent - Commit failed for AdvocateApplication {0} email update", request.ApplicationId);
			return Unit.Value;
		}

		public async Task<bool> Handle(DeclineAdvocateCommand request, CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateApplicationId);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"The advocate application {request.AdvocateApplicationId} cannot be found.", (int)HttpStatusCode.NotFound));
				return false;
			}

			if (advocateApplication.ApplicationStatus != AdvocateApplicationStatus.New)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
					$"The application {request.AdvocateApplicationId} must be in status {AdvocateApplicationStatus.New} for this operation."));
				return false;
			}

			advocateApplication.Decline();

			_advocateApplicationRepository.Update(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationDeclinedEvent(request.AdvocateApplicationId));
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Could not commit the DeclineAdvocateCommand."));

			return false;
		}

		public async Task<Unit> Handle(ChangeAdvocateApplicationNameCommand request, CancellationToken cancellationToken)
		{
			var advocateApplication = await _advocateApplicationRepository.FindAsync(request.AdvocateApplicationId);

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
						$"The advocate application {request.AdvocateApplicationId} cannot be found.", (int)HttpStatusCode.NotFound));
				return Unit.Value;
			}

			advocateApplication.ChangeName(request.FirstName, request.LastName);
			_advocateApplicationRepository.Update(advocateApplication);

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationNameChangedEvent(request.AdvocateApplicationId));
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to change name of advocate application {request.AdvocateApplicationId}"));
			}

			return Unit.Value;
		}

		public async Task<bool> Handle(SubmitAdvocateApplicationProfileCommand request, CancellationToken cancellationToken)
		{
			var advocateApplication = (await _advocateApplicationRepository
			.GetFullAdvocateApplication(aa => aa.Id == request.ApplicationAnswer.AdvocateApplicationId)).FirstOrDefault();

			if (advocateApplication == null)
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"The advocate application with ID {request.ApplicationAnswer.AdvocateApplicationId} does not exists"));
				return false;
			}

			var applicationAnswer = await _applicationAnswerRepository.GetFirstOrDefaultAsync(
				predicate: aa => aa.AdvocateApplicationId == request.ApplicationAnswer.AdvocateApplicationId && aa.QuestionId == request.ApplicationAnswer.QuestionId,
				include: inc => inc
					.Include(i => i.Answers));

			if (applicationAnswer == null)
			{
				_applicationAnswerRepository.Insert(request.ApplicationAnswer);
			}
			else
			{
				var toRemove = advocateApplication.ApplicationAnswers
					.Where(ans => ans.QuestionId == request.ApplicationAnswer.QuestionId)
					.FirstOrDefault();

				advocateApplication.ApplicationAnswers.Remove(toRemove);
				advocateApplication.ApplicationAnswers.Add(request.ApplicationAnswer);
				_advocateApplicationRepository.Update(advocateApplication);
			}

			if (await Commit())
			{
				await _mediator.RaiseEvent(new AdvocateApplicationProfileSubmittedEvent(request.ApplicationAnswer.AdvocateApplicationId, request.ApplicationAnswer.QuestionId));
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(request.MessageType,
				"Could not save advocate profile answers."));

			return false;
		}

		public async Task<Unit> Handle(UpdateSuperSolverCommand request, CancellationToken cancellationToken)
		{
			_advocateApplicationRepository.Insert(request.AdvocateApplications);
			await Commit();
			return Unit.Value;
		}
	}
}