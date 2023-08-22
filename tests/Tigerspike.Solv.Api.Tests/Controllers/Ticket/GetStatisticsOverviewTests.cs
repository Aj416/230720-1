using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Moq;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Core.Constants;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.Ticket
{
	public class GetStatisticsOverviewTests : BaseController<TicketController>
	{

		[Fact]
		public async void ShouldReturnForbiddenWhenUnsupportedRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Customer))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(new Models.TicketStatisticsRequestModel() { });

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
			var response = await SystemUnderTest.GetStatisticsOverview(new Models.TicketStatisticsRequestModel() { });

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForAll(null, null), Times.Once);
		}

		[Fact]
		public async void ShouldQueryDataInDateRangeWhenInAdminRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Admin))
				.Returns(true);

			CultureInfo provider = CultureInfo.InvariantCulture;
			var dateFormat = "ddd dd MMM yyyy h:mm tt zzz";
			var from = DateTime.ParseExact("Tue 11 Feb 2020 8:00 AM -06:00", dateFormat, provider);
			var to = DateTime.ParseExact("Tue 11 Feb 2020 9:00 PM -06:00", dateFormat, provider);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(
					new Models.TicketStatisticsRequestModel()
					{
						From = from,
						To = to
					}
			);

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForAll(from, to), Times.Once);
		}


		[Fact]
		public async void ShouldQueryBrandDataWhenInClientRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Client))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(new Models.TicketStatisticsRequestModel());

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForBrand(It.IsAny<Guid>(), null, null), Times.Once);
		}

		[Fact]
		public async void ShouldQueryDataInDateRangeWhenInClientRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Client))
				.Returns(true);

			CultureInfo provider = CultureInfo.InvariantCulture;
			var dateFormat = "ddd dd MMM yyyy h:mm tt zzz";
			var from = DateTime.ParseExact("Tue 11 Feb 2020 8:00 AM -06:00", dateFormat, provider);
			var to = DateTime.ParseExact("Tue 11 Feb 2020 9:00 PM -06:00", dateFormat, provider);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(
					new Models.TicketStatisticsRequestModel()
					{
						From = from,
						To = to
					}
			);

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForBrand(It.IsAny<Guid>(), from, to), Times.Once);
		}

		[Fact]
		public async void ShouldQueryAdvocateDataWhenInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(true);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(new Models.TicketStatisticsRequestModel() { });

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForAdvocate(It.IsAny<Guid>(), null, null), Times.Once);
		}

		[Fact]
		public async void ShouldQueryDataInDateRangeWhenInAdvocateRole()
		{
			// Arrange
			PrincipalMock
				.Setup(x => x.IsInRole(SolvRoles.Advocate))
				.Returns(true);

			// Act
			CultureInfo provider = CultureInfo.InvariantCulture;
			var dateFormat = "ddd dd MMM yyyy h:mm tt zzz";
			var from = DateTime.ParseExact("Tue 11 Feb 2020 8:00 AM -06:00", dateFormat, provider);
			var to = DateTime.ParseExact("Tue 11 Feb 2020 9:00 PM -06:00", dateFormat, provider);

			// Act
			var response = await SystemUnderTest.GetStatisticsOverview(
					new Models.TicketStatisticsRequestModel()
					{
						From = from,
						To = to
					}
			);

			// Assert
			Mocker.GetMock<ITicketService>().Verify(x => x.GetStatisticsOverviewForAdvocate(It.IsAny<Guid>(), from, to), Times.Once);
		}

	}
}