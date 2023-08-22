using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Events;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateCommandHandlerTests
{
	public class CreateAdvocateIdentityCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenSigningUp()
		{
			// Arrange
			var advocate = GetMockAdvocate();
			var cmd = new CreateAdvocateIdentityCommand(advocate.Id, advocate.FirstName, advocate.LastName, advocate.Email, advocate.Phone, "UK", "Facebook", false, "123qweASD");

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await AdvocateCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockAuthenticationService.Verify(m => m.CreateAdvocate(advocate.Id, advocate.FirstName, advocate.LastName, advocate.Email, cmd.Password), Times.Once);

			MockMediator.Verify(m => m.RaiseEvent(It.Is<AdvocateIdentityCreatedEvent>(dn => dn.UserId == advocate.Id && dn.FirstName == advocate.FirstName && advocate.LastName == dn.LastName && dn.Email == advocate.Email)), Times.Once);

			// There should be no domain notification (because it mainly means there is an error).
			MockMediator.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}
	}
}