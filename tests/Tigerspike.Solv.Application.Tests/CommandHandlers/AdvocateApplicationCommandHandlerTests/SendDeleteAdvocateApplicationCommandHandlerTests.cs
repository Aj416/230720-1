using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Tigerspike.Solv.Core;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Messaging.Notification;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class SendDeleteAdvocateApplicationCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenDeleting()
		{
			// Arrange
			var advocateApplication = GetMockAdvocateApplication();
			var serviceSettings = MockServiceSettings.Object.Value;

			var cmd = new SendDeleteAdvocateApplicationCommand(advocateApplication.Email);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockAdvocateApplicationRepository.Setup(ur =>
				ur.Update(It.Is<IEnumerable<AdvocateApplication>>(apps =>
					apps.Single().Id == advocateApplication.Id)));

			MockSendEndpoint.Setup(x =>
					x.Send<ISendEmailMessageCommand>(It.IsAny<ISendEmailMessageCommand>(),
						It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			MockServiceSettings.Setup(ss => ss.Value).Returns(serviceSettings);

			// Act
			var result = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(result);

			MockAdvocateApplicationRepository.Verify(
				es => es.Update(It.Is<AdvocateApplication>(apps =>
					apps.Id == advocateApplication.Id)), Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Never);
		}

		[Fact]
		public async Task ShouldFailWhenDeleting()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>
				.CreateNew()
				.With(a => a.Id, Guid.NewGuid())
				.With(a => a.DeletionHash, new string('0', 64))
				.With(a => a.Email, "test@email.com")
				.Build();

			var serviceSettings = MockServiceSettings.Object.Value;

			var cmd = new SendDeleteAdvocateApplicationCommand(advocateApplication.Email);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
					It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
					It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
					It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			MockAdvocateApplicationRepository.Setup(ur =>
				ur.Update(It.Is<IEnumerable<AdvocateApplication>>(apps =>
					apps.Single().Id == advocateApplication.Id)));

			MockSendEndpoint.Setup(x =>
					x.Send(It.IsAny<ISendEmailMessageCommand>(),
						It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			MockServiceSettings.Setup(ss => ss.Value).Returns(serviceSettings);

			// Act
			var result = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.False(result);

			MockAdvocateApplicationRepository.Verify(
				es => es.Update(It.Is<AdvocateApplication>(apps =>
					apps.Id == advocateApplication.Id)), Times.Once);

			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Once);
		}
	}
}