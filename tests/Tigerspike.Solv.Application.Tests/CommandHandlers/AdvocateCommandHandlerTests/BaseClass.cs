using System;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateCommandHandlerTests
{
	public class BaseClass
	{
		protected readonly AdvocateCommandHandler AdvocateCommandHandler;
		protected readonly Mock<IAdvocateRepository> MockAdvocateRepository;
		protected readonly Mock<IBrandRepository> MockBrandRepository;
		protected readonly Mock<IAdvocateBrandRepository> MockAdvocateBrandRepository;
		protected readonly Mock<ITicketRepository> MockTicketRepository;
		protected readonly Mock<IAdvocateApplicationRepository> MockAdvocateApplicationRepository;
		protected readonly Mock<IQuizAdvocateAttemptRepository> MockQuizAdvocateAttemptRepository;
		protected readonly Mock<IQuestionRepository> MockQuestionRepository;
		protected readonly Mock<IUserRepository> MockUserRepository;
		protected readonly Mock<IPaymentService> MockPaymentService;
		protected readonly Mock<IOptions<EmailTemplatesOptions>> MockEmailTemplatesOptions;
		protected readonly Mock<IUnitOfWork> MockUnitOfWork;
		protected readonly Mock<IMediatorHandler> MockMediator;
		protected readonly Mock<IDomainNotificationHandler> MockNotificationHandler;
		protected readonly Mock<IAuthenticationService> MockAuthenticationService;
		protected readonly Mock<IWebHostEnvironment> MockHostingEnvironment;
		protected readonly Mock<IBus> MockBus;
		protected readonly Mock<ITimestampService> MockTimestampService;
		protected readonly Mock<ILogger<AdvocateCommandHandler>> MockLogger;
		private readonly Mock<IFeatureManager> MockFeatureManager;

		protected BaseClass()
		{
			MockTicketRepository = new Mock<ITicketRepository>();
			MockAdvocateRepository = new Mock<IAdvocateRepository>();
			MockBrandRepository = new Mock<IBrandRepository>();
			MockAdvocateBrandRepository = new Mock<IAdvocateBrandRepository>();
			MockAdvocateApplicationRepository = new Mock<IAdvocateApplicationRepository>();
			MockQuizAdvocateAttemptRepository = new Mock<IQuizAdvocateAttemptRepository>();
			MockQuestionRepository = new Mock<IQuestionRepository>();
			MockUserRepository = new Mock<IUserRepository>();
			MockPaymentService = new Mock<IPaymentService>();
			MockEmailTemplatesOptions = new Mock<IOptions<EmailTemplatesOptions>>();
			MockUnitOfWork = new Mock<IUnitOfWork>();
			MockMediator = new Mock<IMediatorHandler>();
			MockNotificationHandler = new Mock<IDomainNotificationHandler>();
			MockAuthenticationService = new Mock<IAuthenticationService>();
			MockHostingEnvironment = new Mock<IWebHostEnvironment>();
			MockBus = new Mock<IBus>();
			MockTimestampService = new Mock<ITimestampService>();
			MockLogger = new Mock<ILogger<AdvocateCommandHandler>>();
			MockFeatureManager = new Mock<IFeatureManager>();

			MockEmailTemplatesOptions.Setup(s => s.Value).Returns(new EmailTemplatesOptions
			{
				AdvocateExportEmailSubject = "subject",
				EmailLogoLocation = "location",
				AdvocateExportEmailAttachmentContentType = "type",
				AdvocateExportEmailAttachmentFileName = "filename"
			});

			AdvocateCommandHandler = new AdvocateCommandHandler(
				MockAdvocateRepository.Object,
				MockAdvocateBrandRepository.Object,
				MockTicketRepository.Object,
				MockBrandRepository.Object,
				MockAdvocateApplicationRepository.Object,
				MockQuizAdvocateAttemptRepository.Object,
				MockQuestionRepository.Object,
				MockUserRepository.Object,
				MockPaymentService.Object,
				MockAuthenticationService.Object,
				MockHostingEnvironment.Object,
				MockUnitOfWork.Object,
				MockMediator.Object,
				MockTimestampService.Object,
				MockNotificationHandler.Object,
				MockLogger.Object,
				MockFeatureManager.Object);
		}


		protected static User GetMockAdvocate()
		{
			return new User(
				Guid.NewGuid(),
				"Joanna",
				"Dance",
				"Joanna@nowhere.com",
				"+123 456 789");
		}
	}
}