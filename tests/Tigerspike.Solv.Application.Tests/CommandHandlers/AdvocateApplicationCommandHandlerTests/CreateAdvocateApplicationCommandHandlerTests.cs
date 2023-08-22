using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class CreateAdvocateApplicationCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenInsertedIntoDatabase()
		{
			// Arrange
			var expectedApp = GetMockAdvocateApplication();
			var cmd = new CreateAdvocateApplicationCommand(expectedApp.Country, expectedApp.State, expectedApp.Email,
				expectedApp.FirstName, expectedApp.LastName, expectedApp.Phone, "facebook", "on", "on", false, expectedApp.Address, expectedApp.City, expectedApp.ZipCode, "on", "test@123");

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockAdvocateApplicationRepository.Verify(m =>
					m.InsertAsync(It.Is<AdvocateApplication>(
						actualApp => actualApp.FirstName == expectedApp.FirstName
									 && actualApp.LastName == expectedApp.LastName
									 && actualApp.Phone == expectedApp.Phone
									 && actualApp.Email == expectedApp.Email
									 && actualApp.Country == expectedApp.Country), It.IsAny<CancellationToken>())
				, Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);

			MockMediator.Verify(
				m => m.RaiseEvent(It.IsAny<AdvocateApplicationCreatedEvent>()),
				Times.Once);
		}

		[Fact]
		public async Task ShouldFailWhenInsertingIntoDatabase()
		{
			// Arrange
			var expectedApp = GetMockAdvocateApplication();
			var cmd = new CreateAdvocateApplicationCommand(expectedApp.Country, expectedApp.State, expectedApp.Email,
				expectedApp.FirstName, expectedApp.LastName, expectedApp.Phone, "facebook", "on", "on", false, expectedApp.Address, expectedApp.City, expectedApp.ZipCode, "on", "test@123");

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockAdvocateApplicationRepository.Verify(m =>
					m.InsertAsync(It.Is<AdvocateApplication>(
						actualApp => actualApp.FirstName == expectedApp.FirstName
									 && actualApp.LastName == expectedApp.LastName
									 && actualApp.Phone == expectedApp.Phone
									 && actualApp.Email == expectedApp.Email
									 && actualApp.Country == expectedApp.Country), It.IsAny<CancellationToken>())
				, Times.Once);

			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Once);
		}
	}
}