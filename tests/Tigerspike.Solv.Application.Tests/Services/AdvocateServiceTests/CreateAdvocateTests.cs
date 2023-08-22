using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateServiceTests
{
	public class CreateAdvocateTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenCreatingAdvocate()
		{
			//Arrange
			var advocateApplication = new AdvocateApplication("Sheesa", "Hore", "sheesa@email.com", "+123 456 789",
				"ca", "us", "facebook", false, "102, Down Street", "Los Angeles", "10001");
			Mediator.Setup(m => m.SendCommand(It.IsAny<CreateAdvocateIdentityCommand>())).ReturnsAsync(Unit.Value);

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
				It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
				It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(advocateApplication);

			var mockAdvocate = Builder<Advocate>.CreateNew()
				.WithFactory(() => new Advocate(Builder<User>.CreateNew().Build(), "us", "Facebook", false, false, false))
				.Build();

			MockAdvocateRepository.Setup(m => m.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<Advocate, Advocate>>>(),
				It.IsAny<Expression<Func<Advocate, bool>>>(),
				It.IsAny<Func<IQueryable<Advocate>, IOrderedQueryable<Advocate>>>(),
				It.IsAny<Func<IQueryable<Advocate>, IIncludableQueryable<Advocate, object>>>(),
				It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(mockAdvocate);

			//Act
			await AdvocateService.Create("token", "password");

			//Assert
			Mediator.Verify(m => m.SendCommand(It.IsAny<CreateAdvocateIdentityCommand>()), Times.Exactly(1));
		}
	}
}