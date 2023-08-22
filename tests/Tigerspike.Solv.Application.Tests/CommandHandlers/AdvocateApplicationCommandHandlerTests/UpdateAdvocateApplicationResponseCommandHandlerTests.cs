using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class UpdateAdvocateApplicationResponseCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenUpdating()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew().Build();
			var cmd = new SetAdvocateApplicationResponseEmailSentCommand(advocateApplication.Id);

			MockAdvocateApplicationRepository.Setup(ur => ur.FindAsync(It.IsAny<Guid>()))
				.ReturnsAsync(advocateApplication);
			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockUnitOfWork.Verify(aar => aar.SaveChangesAsync(It.IsAny<CancellationToken>()),
				Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Never);
		}

		[Fact]
		public async Task ShouldFailWhenUpdating()
		{
			// Arrange
			var advocateApplication = Builder<AdvocateApplication>.CreateNew().Build();
			var cmd = new SetAdvocateApplicationResponseEmailSentCommand(advocateApplication.Id);

			MockAdvocateApplicationRepository.Setup(ur => ur.FindAsync(It.IsAny<Guid>()))
				.ReturnsAsync((AdvocateApplication)null);

			// Act
			await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockUnitOfWork.Verify(aar => aar.SaveChangesAsync(It.IsAny<CancellationToken>()),
				Times.Never);

			MockMediator.Verify(m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == cmd.MessageType)),
				Times.Once);
		}
	}
}