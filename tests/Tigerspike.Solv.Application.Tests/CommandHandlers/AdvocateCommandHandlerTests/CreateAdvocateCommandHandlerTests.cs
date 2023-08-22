using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateCommandHandlerTests
{
	public class CreateAdvocateCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenCreating()
		{
			// Arrange
			var advocate = GetMockAdvocate();
			var cmd = new CreateAdvocateCommand(advocate.Id, advocate.FirstName, advocate.LastName, advocate.Email, advocate.Phone, "UK", "Facebook", false, false, false);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
			MockAdvocateApplicationRepository.Setup(aar => aar.FindAsync(It.IsAny<Guid>())).ReturnsAsync(Builder<AdvocateApplication>.CreateNew().Build());

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockAdvocateRepository.Verify(m =>
					m.InsertAsync(It.Is<Advocate>(
						actualApp => actualApp.Id == advocate.Id
									 && actualApp.User.Email == advocate.Email
									 && actualApp.User.FirstName == advocate.FirstName
									 && actualApp.User.LastName == advocate.LastName), It.IsAny<CancellationToken>())
				, Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}

		[Fact]
		public async Task ShouldFailWhenCreating()
		{
			// Arrange
			var advocate = GetMockAdvocate();
			var cmd = new CreateAdvocateCommand(advocate.Id, advocate.FirstName, advocate.LastName, advocate.Email, advocate.Phone, "UK", "Facebook", false, false, false);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));
			MockAdvocateApplicationRepository.Setup(aar => aar.FindAsync(It.IsAny<Guid>())).ReturnsAsync(Builder<AdvocateApplication>.CreateNew().Build());

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockAdvocateRepository.Verify(m =>
					m.InsertAsync(It.Is<Advocate>(
						actualApp => actualApp.User.Id == advocate.Id
									 && actualApp.User.Email == advocate.Email
									 && actualApp.User.FirstName == advocate.FirstName
									 && actualApp.User.LastName == advocate.LastName), It.IsAny<CancellationToken>())
				, Times.Once);

			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Once);
		}
	}
}