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
	public class DeleteWhiteListPhrasesBrandControllerTests : BaseController<BrandController>
	{
		[Fact]
		public async void ShouldReturn200WhenDataIsDeleted()
		{
			// Arrange
			string[] whiteListPhrases = { "Phrase 1", "Phrase 2" };
			var brandId = Guid.NewGuid();

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.DeleteWhitelistPhrase(brandId, whiteListPhrases))
				.ReturnsAsync(new string[] {
					"Phrase 1","Phrase 2"
				});

			// Act
			var result = await SystemUnderTest.BlacklistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, whiteListPhrases);
		}

		[Fact]
		public async void ShouldReturn200WhenNoData()
		{
			// Arrange
			string[] whiteListPhrases = { "Phrase 1", "Phrase 2" };
			string[] wlPhrase = { };
			var brandId = Guid.NewGuid();

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.DeleteWhitelistPhrase(brandId, whiteListPhrases))
				.ReturnsAsync(new string[] { });

			// Act
			var result = await SystemUnderTest.BlacklistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, wlPhrase);
		}

		[Fact]
		public async void ShouldReturn200WhenContainsDeletedData()
		{
			// Arrange
			string[] whiteListPhrases = { "Phrase 1", "Phrase 2", "Phrase 3" };
			string[] wlPhrase = { "Phrase 2" };
			var brandId = Guid.NewGuid();

			Mocker.GetMock<IBrandService>()
				.Setup(x => x.DeleteWhitelistPhrase(brandId, whiteListPhrases))
				.ReturnsAsync(new string[] { "Phrase 2" });

			// Act
			var result = await SystemUnderTest.BlacklistPhrase(brandId, whiteListPhrases);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, wlPhrase);
		}
	}
}