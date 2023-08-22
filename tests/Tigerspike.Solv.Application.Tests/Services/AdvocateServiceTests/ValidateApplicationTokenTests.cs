using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateServiceTests
{
	public class ValidateApplicationTokenTests : BaseClass
	{
		[Fact]
		public async Task ShouldSucceedWhenValidating()
		{
			//Arrange
			var token = "lsjdflkamlwefjlwekjflskdjfwef-ew0432423";

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
				It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
				It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(Builder<AdvocateApplication>.CreateNew().Build());

			//Act
			var result = await AdvocateService.ValidateToken(token);

			//Assert
			Assert.True(result);
		}

		[Fact]
		public async Task ShouldFailWhenValidating()
		{
			//Arrange
			var token = "lsjdflkamlwefjlwekjflskdjfwef-ew0432423";

			MockAdvocateApplicationRepository.Setup(m => m.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<AdvocateApplication, AdvocateApplication>>>(),
				It.IsAny<Expression<Func<AdvocateApplication, bool>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IOrderedQueryable<AdvocateApplication>>>(),
				It.IsAny<Func<IQueryable<AdvocateApplication>, IIncludableQueryable<AdvocateApplication, object>>>(),
				It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(null as AdvocateApplication);

			//Act
			var result = await AdvocateService.ValidateToken(token);

			//Assert
			Assert.False(result);
		}
	}
}