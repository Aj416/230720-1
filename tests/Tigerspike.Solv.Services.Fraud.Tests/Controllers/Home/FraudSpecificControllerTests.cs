using Moq;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Controllers;
using Tigerspike.Solv.Services.Fraud.Models;
using Xunit;
using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Services.Fraud.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Tigerspike.Solv.Services.Fraud.Tests.Controllers.Home
{
	public class FraudSpecificControllerTests : BaseController<HomeController>
	{
		[Fact]
		public async void ShouldReturn200WhenSearchResultsFound()
		{
			// Arrange
			var frauds = new List<FraudSearchModel>(){
				Builder<FraudSearchModel>.CreateNew()
				.With(x => x.BrandName, "BrandName1")
				.With(x => x.FraudStatus, FraudStatus.FraudSuspected)
				.With(x => x.Level, TicketLevel.L1)
				.With(x => x.AdvocateName, "AdvocateName1")
				.With(x => x.FraudLevel, 3)
				.Build(),
				Builder<FraudSearchModel>.CreateNew()
				.With(x => x.BrandName, "BrandName2")
				.With(x => x.FraudStatus, FraudStatus.FraudSuspected)
				.With(x => x.Level, TicketLevel.L1)
				.With(x => x.AdvocateName, "AdvocateName2")
				.With(x => x.FraudLevel, 2)
				.Build(),
				Builder<FraudSearchModel>.CreateNew()
				.With(x => x.BrandName, "BrandName3")
				.With(x => x.FraudStatus, FraudStatus.FraudSuspected)
				.With(x => x.Level, TicketLevel.L1)
				.With(x => x.AdvocateName, "AdvocateName3")
				.With(x => x.FraudLevel, 2)
				.Build()
			};

			var request = Builder<FraudSearchCriteriaModel>.CreateNew()
				.With(x => x.Statuses, FraudStatus.FraudSuspected)
				.Build();

			Mocker.GetMock<ISearchService<FraudSearchModel>>()
				.Setup(x => x.Search(It.Is<FraudSearchCriteriaModel>(x => x.Statuses == FraudStatus.FraudSuspected)))
				.ReturnsAsync(new PagedList<FraudSearchModel>(frauds, 0, 10, 0));

			// Act
			var result = await SystemUnderTest.FraudSearch(request);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async void ShouldReturn204henSearchResultsNotFound()
		{
			var request = Builder<FraudSearchCriteriaModel>.CreateNew()
				.Build();

			Mocker.GetMock<ISearchService<FraudSearchModel>>()
				.Setup(x => x.Search(It.Is<FraudSearchCriteriaModel>(x => x.Statuses == FraudStatus.FraudSuspected)))
				.ReturnsAsync(new PagedList<FraudSearchModel>(new List<FraudSearchModel>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

			// Act
			var result = await SystemUnderTest.FraudSearch(request);

			// Assert
			Assert_StatusCode(StatusCodes.Status204NoContent, result);
			Assert.IsType<NoContentResult>(result);
		}
	}
}