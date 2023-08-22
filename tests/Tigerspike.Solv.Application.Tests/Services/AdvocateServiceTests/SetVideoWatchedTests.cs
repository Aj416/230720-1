using System;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateServiceTests
{
	public class SetVideoWatchedTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenSettingVideoWatchedFlag()
		{
			//Arrange
			var id = Guid.NewGuid();

			//Act
			await AdvocateService.SetVideoWatched(id);

			//Assert
			// The command should be sent with the exact passed parameters to the service
			Mediator.Verify(m => m.SendCommand(It.Is<SetVideoWatchedCommand>(cmd => cmd.AdvocateId == id)), Times.Exactly(1));
		}
	}
}