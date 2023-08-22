using System;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tigerspike.Solv.Services.Fraud.Application.Services;
using Tigerspike.Solv.Services.Fraud.Controllers;
using Tigerspike.Solv.Services.Fraud.Models;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.Controllers.Home
{
	public class TicketSpecificControllerTests : BaseController<HomeController>
	{
		[Fact]
		public async void ShouldReturn200WhenTicketIsFound()
		{
			// Arrange
			var ticketId = Guid.NewGuid();

			var ticketModel = Builder<TicketModel>.CreateNew()
				.With(t => t.TicketId, ticketId)
				.Build();


			Mocker.GetMock<IFraudService>()
				.Setup(x => x.GetTicketDetails(It.IsAny<Guid>()))
				.Returns(ticketModel);

			// Act
			var result = await SystemUnderTest.GetTicket(ticketId);

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, ticketModel);
		}

		[Fact]
		public async void ShouldReturn204WhenTicketIsNotFound()
		{
			// Arrange
			var ticketId = Guid.NewGuid();

			Mocker.GetMock<IFraudService>()
				.Setup(x => x.GetTicketDetails(It.IsAny<Guid>()))
				.Returns((TicketModel)null);

			// Act
			var result = await SystemUnderTest.GetTicket(ticketId);

			// Assert
			Assert_StatusCode(StatusCodes.Status204NoContent, result);
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public void ShouldReturn200()
		{
			// Arrange
			var message = "Solv Fraud Service";

			// Act
			var result = SystemUnderTest.Get();

			// Assert
			Assert_StatusCode(StatusCodes.Status200OK, result);
			Assert.IsType<OkObjectResult>(result);
			var objectResult = (OkObjectResult)result;
			Assert.Equal(objectResult.Value, message);
		}
	}
}