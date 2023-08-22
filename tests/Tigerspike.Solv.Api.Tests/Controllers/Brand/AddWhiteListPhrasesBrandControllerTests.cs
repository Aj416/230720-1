using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.Brand
{
	public class AddWhiteListPhrasesBrandControllerTests : BaseController<BrandController>
	{
		[Fact]
		public async void ShouldReturn200WhenDataIsInserted()
		{
			// Arrange
			string[] whiteListPhrases = { "18004746836", "18448141801" };
			var brandId = Guid.NewGuid();

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.AddWhitelistPhrases(brandId, whiteListPhrases))
				.ReturnsAsync(whiteListPhrases);

			// Act
			var result = await SystemUnderTest.WhitelistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, whiteListPhrases);
		}

		[Fact]
		public async void ShouldReturn200AcceptedDataIsInserted()
		{
			// Arrange
			string[] whiteListPhrases = { "18004746836", "18448141801" };
			var brandId = Guid.NewGuid();
			string[] wlPhrase = { "18004746836" };

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.AddWhitelistPhrases(brandId, whiteListPhrases))
				.ReturnsAsync(wlPhrase);

			// Act
			var result = await SystemUnderTest.WhitelistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, wlPhrase);
		}

		[Fact]
		public async void ShouldReturn200NoDataIsInserted()
		{
			// Arrange
			string[] whiteListPhrases = { "18004746836", "18448141801" };
			var brandId = Guid.NewGuid();
			string[] wlPhrase = { };

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.AddWhitelistPhrases(brandId, whiteListPhrases))
				.ReturnsAsync(wlPhrase);

			// Act
			var result = await SystemUnderTest.WhitelistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, wlPhrase);
		}
	}
}