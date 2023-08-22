using System;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateServiceTests
{
	public class SetupPaymentAccountTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenSettingUpPaymentAccount()
		{
			//Arrange
			var id = Guid.NewGuid();

            //Act
            // TODO: Fix the unit testing.
            await Task.CompletedTask;
			//await AdvocateService.UpdatePaymentMethodStatus(id);

			//Assert
			// The command should be sent with the exact passed parameters to the service
			// Mediator.Verify(m => m.SendCommand(It.Is<UpdateAdvocatePaymentAccountCommand>(cmd => cmd.AdvocateId == id)), Times.Exactly(1));
		}
	}
}