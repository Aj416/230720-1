using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Infra.Bus.Extensions;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Notification;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class BaseClass
	{
		protected readonly AdvocateApplicationCommandHandler AdvocateApplicationCommandHandler;
		protected readonly Mock<IAdvocateApplicationRepository> MockAdvocateApplicationRepository;
		protected readonly Mock<IApplicationAnswerRepository> MockApplicationAnswerRepository;
		protected readonly Mock<IBus> MockBus;
		protected readonly IOptions<BusOptions> MockBusOptions;
		protected readonly Mock<IOptions<EmailTemplatesOptions>> MockServiceSettings;
		protected readonly Mock<IUnitOfWork> MockUnitOfWork;
		protected readonly Mock<IMediatorHandler> MockMediator;
		protected readonly Mock<IDomainNotificationHandler> MockNotificationHandler;
		protected readonly Mock<ISendEndpoint> MockSendEndpoint;
		protected readonly Mock<ILogger<AdvocateApplicationCommandHandler>> MockLogger;
		private readonly Mock<IAdvocateService> MockAdvocateService;
		private readonly Mock<INewProfileService> MockNewProfileService;

		protected BaseClass()
		{
			MockAdvocateApplicationRepository = new Mock<IAdvocateApplicationRepository>();
			MockApplicationAnswerRepository = new Mock<IApplicationAnswerRepository>();
			MockSendEndpoint = new Mock<ISendEndpoint>();
			MockLogger = new Mock<ILogger<AdvocateApplicationCommandHandler>>();

			MockBus = new Mock<IBus>();
			MockBus.Setup(x => x.GetSendEndpoint(It.IsAny<Uri>())).Returns(Task.FromResult(MockSendEndpoint.Object));
			EndpointConvention.Map<ISendEmailMessageCommand>(new Uri("http://random"));

			MockBusOptions =  Options.Create(new BusOptions());;
			MockServiceSettings = new Mock<IOptions<EmailTemplatesOptions>>();
			MockUnitOfWork = new Mock<IUnitOfWork>();
			MockMediator = new Mock<IMediatorHandler>();
			MockNotificationHandler = new Mock<IDomainNotificationHandler>();
			MockAdvocateService = new Mock<IAdvocateService>();
			MockNewProfileService = new Mock<INewProfileService>();

			MockServiceSettings.Setup(s => s.Value).Returns(new EmailTemplatesOptions
			{
				AdvocateExportEmailSubject = "subject",
				EmailLogoLocation = "location",
				AdvocateExportEmailAttachmentContentType = "type",
				AdvocateExportEmailAttachmentFileName = "filename",
				AdvocateSignUpUrl = "{0}"
			});

			AdvocateApplicationCommandHandler = new AdvocateApplicationCommandHandler(
				MockAdvocateService.Object,
				MockAdvocateApplicationRepository.Object,
				MockApplicationAnswerRepository.Object,
				MockServiceSettings.Object,
				MockUnitOfWork.Object,
				MockMediator.Object,
				MockBus.Object,
				MockBusOptions,
				MockLogger.Object,
				MockNotificationHandler.Object,
				MockNewProfileService.Object);
		}

		protected static AdvocateApplication GetMockAdvocateApplication()
		{
			return new AdvocateApplication("Joanna", "Dance", "Joanna@nowhere.com", "+123 456 789", "CA", "US",
				"facebook", false, "102, Down Street", "Los Angeles", "10001");
		}
	}
}