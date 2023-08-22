using System;
using Microsoft.AspNetCore.Http;
using Moq;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Constants;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.Ticket
{
	public class GetStatisticsByStatusTests : BaseController<TicketController>
	{

		[Fact]
		public async void ShouldReturnForbiddenWhenUnsupportedRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Customer))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetTicketStatisticsByStatus();

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
			var response = await SystemUnderTest.GetTicketStatisticsByStatus();

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsByStatusForAll(), Times.Once);
		}

		[Fact]
		public async void ShouldQueryBrandDataWhenInClientRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Client))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetTicketStatisticsByStatus();

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsByStatusForBrand(It.IsAny<Guid>()), Times.Once);
		}

		[Fact]
		public async void ShouldQueryAdvocateDataWhenInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetTicketStatisticsByStatus();

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsByStatusForAdvocate(It.IsAny<Guid>()), Times.Once);
		}

	}
}