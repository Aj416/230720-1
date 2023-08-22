using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Validators
{
	public class HandlingImportConsumerTests : ImportConsumerTestsBase
	{

		[Fact]
		public async Task WhenImportTicketCommandIsInvalidItShouldRaiseFailure()
		{
			var importId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);
			var ctxMock = Mocker.GetMock<ConsumeContext<ImportTicketCommand>>();
			ctxMock.Setup(x => x.Message).Returns(cmd);

			Mocker.GetMock<IValidator<ImportTicketCommand>>()
				.Setup(x => x.ValidateAsync(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult(new [] { new ValidationFailure(nameof(ImportTicketCommand.ImportId), "invalid-import-id")}));

			var mediatorMock = Mocker.GetMock<IMediator>();

			var serviceScopeMock = Mocker.GetMock<IServiceScope>();
			serviceScopeMock
				.Setup(x => x.ServiceProvider.GetService(typeof(IMediator)))
				.Returns(mediatorMock.Object);

			Mocker.GetMock<IServiceScopeFactory>()
				.Setup(x => x.CreateScope())
				.Returns(serviceScopeMock.Object);

			await SystemUnderTest.Consume(ctxMock.Object);

			mediatorMock.Verify(x => x.Send(It.Is<AddTicketImportFailureCommand>(x => x.ImportId == importId && x.FailureReason == "invalid-import-id"), It.IsAny<CancellationToken>()), Times.Once);
		}		

		[Fact]
		public async Task WhenImportTicketCommandHandlerThrowsAnExceptionItShouldRaiseFailure()
		{
			var importId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);
			var ctxMock = Mocker.GetMock<ConsumeContext<ImportTicketCommand>>();
			ctxMock.Setup(x => x.Message).Returns(cmd);

			Mocker.GetMock<IValidator<ImportTicketCommand>>()
				.Setup(x => x.ValidateAsync(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			var mediatorMock = Mocker.GetMock<IMediator>();
			mediatorMock
				.Setup(x => x.Send(cmd, It.IsAny<CancellationToken>()))
				.ThrowsAsync(new System.Exception("exception-test"));

			var serviceScopeMock = Mocker.GetMock<IServiceScope>();
			serviceScopeMock
				.Setup(x => x.ServiceProvider.GetService(typeof(IMediator)))
				.Returns(mediatorMock.Object);

			Mocker.GetMock<IServiceScopeFactory>()
				.Setup(x => x.CreateScope())			
				.Returns(serviceScopeMock.Object);

			await SystemUnderTest.Consume(ctxMock.Object);

			mediatorMock.Verify(x => x.Send(It.Is<AddTicketImportFailureCommand>(x => x.ImportId == importId && x.FailureReason == "exception-test"), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task WhenImportTicketCommandHandlerFailsItShouldRaiseFailure()
		{
			var importId = Guid.NewGuid();
			var cmd = ImportTicketCommandBuilder.Build(importId: importId);
			var ctxMock = Mocker.GetMock<ConsumeContext<ImportTicketCommand>>();
			ctxMock.Setup(x => x.Message).Returns(cmd);

			Mocker.GetMock<IValidator<ImportTicketCommand>>()
				.Setup(x => x.ValidateAsync(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			Mocker.GetMock<IDomainNotificationHandler>()
				.Setup(x => x.GetNotifications())
				.Returns(new [] { new DomainNotification("key", "sample-failure-reason") });				

			var mediatorMock = Mocker.GetMock<IMediator>();
			mediatorMock
				.Setup(x => x.Send(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(null as Guid?);

			var serviceScopeMock = Mocker.GetMock<IServiceScope>();
			serviceScopeMock
				.Setup(x => x.ServiceProvider.GetService(typeof(IMediator)))
				.Returns(mediatorMock.Object);

			Mocker.GetMock<IServiceScopeFactory>()
				.Setup(x => x.CreateScope())
				.Returns(serviceScopeMock.Object);

			await SystemUnderTest.Consume(ctxMock.Object);

			mediatorMock.Verify(x => x.Send(It.Is<AddTicketImportFailureCommand>(x => x.ImportId == importId && x.FailureReason == "sample-failure-reason"), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task WhenImportTicketCommandIsHandledProperlyItShouldNotRaiseFailure()
		{
			var cmd = ImportTicketCommandBuilder.Build();
			var ctxMock = Mocker.GetMock<ConsumeContext<ImportTicketCommand>>();
			ctxMock.Setup(x => x.Message).Returns(cmd);

			Mocker.GetMock<IValidator<ImportTicketCommand>>()
				.Setup(x => x.ValidateAsync(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			var mediatorMock = Mocker.GetMock<IMediator>();
			mediatorMock
				.Setup(x => x.Send(cmd, It.IsAny<CancellationToken>()))
				.ReturnsAsync(Guid.NewGuid());

			var serviceScopeMock = Mocker.GetMock<IServiceScope>();
			serviceScopeMock
				.Setup(x => x.ServiceProvider.GetService(typeof(IMediator)))
				.Returns(mediatorMock.Object);

			Mocker.GetMock<IServiceScopeFactory>()
				.Setup(x => x.CreateScope())
				.Returns(serviceScopeMock.Object);

			await SystemUnderTest.Consume(ctxMock.Object);

			mediatorMock.Verify(x => x.Send(It.IsAny<AddTicketImportFailureCommand>(), It.IsAny<CancellationToken>()), Times.Never);
		}

	}
}