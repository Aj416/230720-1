using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Models.Profile;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class SubmittedAnswersTests : BaseClass
	{
		[Fact]
		public async Task ShouldBeInvalidWhenValidatingAnswers()
		{
			// Arrange
			MockQuestionRepository.Setup(m => m.FindAsync(It.IsAny<Guid>())).ReturnsAsync(null as Question);

			// Act
			var validateData = await AdvocateApplicationService.ValidateAnswers(InvalidAnswers);

			// Act
			Assert.True(validateData.Any());
		}
	}
}