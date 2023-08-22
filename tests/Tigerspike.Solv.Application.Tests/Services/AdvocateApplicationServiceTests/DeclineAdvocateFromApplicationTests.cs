using System;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class DeclineAdvocateFromApplicationTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenDecliningAdvocateApplication()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<DeclineAdvocateCommand>())).ReturnsAsync(true);

			//Act
			await AdvocateApplicationService.DeclineAdvocateApplication(new[] { Guid.NewGuid() });

			//Assert
			Mediator.Verify(db => db.SendCommand(It.IsAny<DeclineAdvocateCommand>()), Times.Exactly(1));
		}

		[Fact]
		public async void ShouldReturnFalseWhenDecliningAdvocateApplication()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<DeclineAdvocateCommand>())).ReturnsAsync(false);

			//Act
			await AdvocateApplicationService.DeclineAdvocateApplication(new [] { Guid.NewGuid() });

			//Assert
			Mediator.Verify(db => db.SendCommand(It.IsAny<DeclineAdvocateCommand>()), Times.Exactly(1));
		}
	}
}