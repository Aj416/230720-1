using System;
using Microsoft.AspNetCore.Http;
using Moq;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Constants;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.Advocate
{
	public class GetAdvocateStatisticsByStatusTests : BaseAdvocateControllerTests
	{

		[Fact]
		public async void ShouldReturnForbiddenWhenInCustomerRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Customer))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetAdvocateStatisticsByStatus();

			// Assert
			Assert_StatusCode(StatusCodes.Status403Forbidden, response);
		}

		[Fact]
		public async void ShouldReturnForbiddenWhenInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetAdvocateStatisticsByStatus();

			// Assert
			Assert_StatusCode(StatusCodes.Status403Forbidden, response);
		}

		[Fact]
		public async void ShouldQueryAllDataWhenInAdminRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Admin))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetAdvocateStatisticsByStatus();

			// Assert
			Mocker.GetMock<IAdvocateService>().Verify(x => x.GetStatisticsByStatusForAll(), Times.Once);
		}

		[Fact]
		public async void ShouldQueryBrandDataWhenInClientRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Client))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetAdvocateStatisticsByStatus();

			// Assert
			Mocker.GetMock<IAdvocateService>().Verify(x => x.GetStatisticsByStatusForBrand(It.IsAny<Guid>()), Times.Once);
		}

	}
}