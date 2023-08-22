using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Tigerspike.Solv.Api.Controllers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Xunit;

namespace Tigerspike.Solv.Api.Tests.Controllers.Brand
{
	public class GetUserUserControllerTests : BaseController<BrandController>
	{
		[Fact]
		public async void ShouldReturn200WhenBrandIsFound()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			Mocker.GetMock<IBrandService>()
				.Setup(x => x.Get(brandId))
				.ReturnsAsync(new BrandModel { Id = brandId });

			// Act
			var response = await SystemUnderTest.GetBrand(brandId);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, response);
			Assert.IsType<OkObjectResult>(response);
			var objectResult = (OkObjectResult)response;
			Assert.IsType<BrandModel>(objectResult.Value);
		}

		// TODO: The controller should be transparent to this, the service should throw a specific exception (like EntityNotFound)
		// that gets intercepted by the global controller filter and handled as 404 (like we do with ServiceException)
		//[Fact]
		//public async void ShouldReturn404NotFoundWhenBrandIsNotFound()
		//{
		//	// Arrange
		//	var brandId = Guid.NewGuid();
		//	Mocker.GetMock<IBrandService>()
		//		.Setup(x => x.Get(brandId))
		//		.ReturnsAsync((BrandModel)null);

		//	// Act
		//	var response = await SystemUnderTest.GetBrand(brandId);

		//	// Assert
		//	Assert_StatusCode(StatusCodes.Status404NotFound, response);
		//	Assert.IsType<NotFoundResult>(response);
		//}
	}
}