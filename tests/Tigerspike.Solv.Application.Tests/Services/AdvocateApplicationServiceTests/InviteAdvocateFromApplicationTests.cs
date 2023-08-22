using System;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class InviteAdvocateFromApplicationTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedInvitingAdvocate()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<InviteAdvocateCommand>())).ReturnsAsync(true);

			//Act
			await AdvocateApplicationService.InviteAdvocateFromApplication(new [] { Guid.NewGuid() });

			//Assert
			Mediator.Verify(db => db.SendCommand(It.IsAny<InviteAdvocateCommand>()), Times.Exactly(1));
		}

		[Fact]
		public async void ShouldReturnFalseInvitingAdvocate()
		{
			//Arrange
			Mediator.Setup(m => m.SendCommand(It.IsAny<InviteAdvocateCommand>())).ReturnsAsync(false);

			//Act
			await AdvocateApplicationService.InviteAdvocateFromApplication(new[] { Guid.NewGuid() });

			//Assert
			Mediator.Verify(db => db.SendCommand(It.IsAny<InviteAdvocateCommand>()), Times.Exactly(1));
		}
	}
}