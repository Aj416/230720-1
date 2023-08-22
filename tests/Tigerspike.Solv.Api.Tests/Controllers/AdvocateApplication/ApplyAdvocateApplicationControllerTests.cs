using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Services;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.AdvocateApplication
{
	public class ApplyAdvocateApplicationControllerTests : BaseController<AdvocateApplicationController>
	{
		private readonly AdvocateApplicationRequestModel _requestModel;

		public ApplyAdvocateApplicationControllerTests()
		{
			// default to happy path
			_requestModel = GetHappyPathRequestModel();
			SetupHappyPathMocks(_requestModel);
		}

		[Fact]
		public async void ShouldReturnBadRequestWhenRecaptchaIsInvalid()
		{
			// Arrange
			Mocker.GetMock<IRecaptchaApiClient>()
				.Setup(x => x.ValidateCaptcha(_requestModel.GoogleRecaptchaResponse))
				.ReturnsAsync(false);

			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status400BadRequest, response);
			Assert.IsType<BadRequestResult>(response);
		}

		[Fact]
		public async void ShouldReturnStatus208AlreadyReportedWhenEmailIsAlreadyUsed()
		{
			// Arrange
			Mocker.GetMock<IAdvocateApplicationService>()
				.Setup(x => x.IsEmailInUse(_requestModel.Email))
				.ReturnsAsync(true);

			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status208AlreadyReported, response);
		}

		[Fact]
		public async void ShouldReturnStatus201CreatedWhenHappyPath()
		{
			// Act
			var response = await Act();

			// Assert
			Assert_StatusCode(StatusCodes.Status201Created, response);
		}

		private static AdvocateApplicationRequestModel GetHappyPathRequestModel()
		{
			return new AdvocateApplicationRequestModel
			{
				Email = "test@tigerspike.com",
				GoogleRecaptchaResponse = Guid.NewGuid().ToString(),
			};
		}

		private void SetupHappyPathMocks(AdvocateApplicationRequestModel model)
		{
			Mocker.GetMock<IRecaptchaApiClient>()
				.Setup(x => x.ValidateCaptcha(model.GoogleRecaptchaResponse))
				.ReturnsAsync(true);

			Mocker.GetMock<IAdvocateApplicationService>()
				.Setup(x => x.IsEmailInUse(model.Email))
				.ReturnsAsync(false);

			Mocker.GetMock<IAdvocateApplicationService>()
				.Setup(x => x.Apply(It.IsAny<AdvocateApplicationModel>()))
				.ReturnsAsync(Guid.NewGuid());
		}

		private async Task<IActionResult> Act()
		{
			return await SystemUnderTest.Apply(_requestModel, Mocker.GetMock<IRecaptchaApiClient>().Object, Mocker.GetMock<INewProfileService>().Object);
		}
	}
}