using FizzWare.NBuilder;
using Xunit;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Application.Tests.Mappings.AdvocateApplicationModel
{
	public class AdvocateApplicationMappingTests : BaseMappingTests
	{
		[Fact]
		public void ShouldBeMappedToModelBasicProperties()
		{
			// Arrange
			var original = Builder<AdvocateApplication>
				.CreateNew()
				.Build();

			// Act
			var result = SystemUnderTest.Map<AdvocateApplication, Application.Models.AdvocateApplicationModel>(original);

			// Assert
			Assert.Equal(original.Email, result.Email);
		}
	}
}