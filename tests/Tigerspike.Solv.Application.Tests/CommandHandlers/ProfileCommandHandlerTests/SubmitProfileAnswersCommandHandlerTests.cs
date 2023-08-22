using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models.Profile;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.ProfileCommandHandlerTests
{
	public class SubmitProfileAnswersCommandHandlerTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenAdvocateHasNotSubmittedAnserwsBefore()
		{
			// Arrange

			var dummyAdvocateApplication = GetMockApplicationAnswer().AdvocateApplication;
			var dummyApplicationAnswerList = new List<ApplicationAnswer>();
			var dummyCmd = new SubmitProfileAnswersCommand(Id, dummyApplicationAnswerList);

			MockAdvocateApplicationRepository.Setup(x => x.FindAsync(Id))
				.ReturnsAsync(dummyAdvocateApplication);

			MockApplicationAnswerRepository
				.Setup(x => x.HasAnswers(dummyCmd.AdvocateApplicationId))
				.ReturnsAsync(false);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act

			var isInserted = await ProfileCommandHandler.Handle(dummyCmd, CancellationToken.None);

			// Assert

			Assert.True(isInserted);
		}

		[Fact]
		public async Task ShouldFailWhenAdvocateHasAlreadySubmittedAnswers()
		{
			// Arrange

			var dummyAdvocateApplication = GetMockApplicationAnswer().AdvocateApplication;
			var dummyApplicationAnswerList = new List<ApplicationAnswer> { GetMockApplicationAnswer() };
			var dummyCmd = new SubmitProfileAnswersCommand(Id, dummyApplicationAnswerList);

			MockAdvocateApplicationRepository.Setup(x => x.FindAsync(Id))
				.ReturnsAsync(dummyAdvocateApplication);

			MockApplicationAnswerRepository
				.Setup(x => x.HasAnswers(dummyCmd.AdvocateApplicationId))
				.ReturnsAsync(true);

			MockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act

			var isInserted = await ProfileCommandHandler.Handle(dummyCmd, CancellationToken.None);

			// Assert

			Assert.False(isInserted);
		}
	}
}