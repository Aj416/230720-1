using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Constants;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.User
{
	public class GetUserUserControllerTests : BaseController<UserController>
	{
		public GetUserUserControllerTests()
		{
			// default to happy path
			SetupHappyPathMocks();
		}

		[Fact]
		public async void ShouldReturnAdvocateModelWhenInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(true);

			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, response);
			Assert.IsType<OkObjectResult>(response);
			var objectResult = (OkObjectResult)response;
			Assert.IsType<AdvocateModel>(objectResult.Value);
		}

		[Fact]
		public async void ShouldReturnUserModelWhenNotInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(false);

			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, response);
			Assert.IsType<OkObjectResult>(response);
			var objectResult = (OkObjectResult)response;
			Assert.IsType<UserModel>(objectResult.Value);
		}

		[Fact]
		public async void ShouldReturn404NotFoundWhenUserIsNotFound()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(false);

			Mocker.GetMock<IUserService>()
				.Setup(x => x.FindByUserId(UserId))
				.ReturnsAsync((UserModel)null);

			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status404NotFound, response);
		}

		private void SetupHappyPathMocks()
		{
			Mocker.GetMock<IUserService>()
				.Setup(x => x.FindByUserId(UserId))
				.ReturnsAsync(new UserModel { Id = UserId });

			Mocker.GetMock<IAdvocateService>()
				.Setup(x => x.FindAsync(UserId, null))
				.ReturnsAsync(new AdvocateModel { Id = UserId });
		}

		private async Task<IActionResult> Act()
		{
			return await SystemUnderTest.GetUser();
		}
	}
}