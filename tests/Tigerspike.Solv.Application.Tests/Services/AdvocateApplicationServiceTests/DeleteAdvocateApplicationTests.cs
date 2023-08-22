using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class DeleteAdvocateApplicationTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenDeletingAdvocateApplication()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<DeleteAdvocateApplicationCommand>())).ReturnsAsync(true);

			//Act
			var result = await AdvocateApplicationService.DeleteAdvocateApplication("test@tigerspike.com", "123");

			//Assert
			Assert.True(result);
			Mediator.Verify(db => db.SendCommand(It.IsAny<DeleteAdvocateApplicationCommand>()), Times.Exactly(1));
		}

		[Fact]
		public async void ShouldReturnFalseWhenDeletingAdvocateApplication()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<DeleteAdvocateApplicationCommand>())).ReturnsAsync(false);

			//Act
			var result = await AdvocateApplicationService.DeleteAdvocateApplication("test@tigerspike.com", "123");

			//Assert
			Assert.False(result);
			Mediator.Verify(db => db.SendCommand(It.IsAny<DeleteAdvocateApplicationCommand>()), Times.Exactly(1));
		}
	}
}