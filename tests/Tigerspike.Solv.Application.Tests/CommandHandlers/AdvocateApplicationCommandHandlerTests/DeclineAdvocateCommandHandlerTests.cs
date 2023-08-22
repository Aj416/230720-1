using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.AdvocateApplicationCommandHandlerTests
{
	public class DeclineAdvocateCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenDeclining()
		{
			// Arrange
			var advocateApplication =
				new AdvocateApplication("John", "Doe", "email", "+123 456 789", "state", "country", "facebook", false, "102, Down Street", "Los Angeles", "10001");

			var cmd = new DeclineAdvocateCommand(advocateApplication.Id);

			MockAdvocateApplicationRepository.Setup(ur => ur.FindAsync(cmd.AdvocateApplicationId))
				.ReturnsAsync(advocateApplication);
			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			var declined = await AdvocateApplicationCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			Assert.True(declined);
		}
	}
}