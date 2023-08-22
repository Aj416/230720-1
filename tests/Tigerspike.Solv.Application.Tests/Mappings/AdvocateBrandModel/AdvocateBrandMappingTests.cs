using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Mappings.AdvocateBrandModel
{
	public class AdvocateBrandMappingTests : BaseMappingTests
	{
		[Fact]
		public void ShouldBeMappedToBaseClassPropertiesInnerBrand()
		{
			// Arrange
			var original = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.ContractAccepted = true)
				.With(x => x.Brand = Brand.CreatePracticeBrand("a", "b", "c"))
				.Build();

			// Act
			var result = SystemUnderTest.Map<Application.Models.AdvocateBrandModel>(original);

			// Assert
			Assert.Equal(original.Brand.Id, result.Id);
			Assert.Equal(original.Brand.Name, result.Name);
			Assert.Equal(original.Brand.Logo, result.Logo);
			Assert.Equal(original.Brand.Thumbnail, result.Thumbnail);
		}

		[Fact]
		public void ShouldBeMappedToModelBasicProperties()
		{
			// Arrange
			var original = Builder<AdvocateBrand>
				.CreateNew()
				.With(x => x.ContractAccepted = true)
				.With(x => x.Authorized = false)
				.Build();

			// Act
			var result = SystemUnderTest.Map<Application.Models.AdvocateBrandModel>(original);

			// Assert
			Assert.Equal(original.ContractAccepted, result.ContractAccepted);
			Assert.Equal(original.Authorized, result.Authorized);
		}
	}
}