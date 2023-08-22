using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.AdvocateBrandRepositoryTests
{
	public class GetIdleCountByBrandTests : BaseRepositoryTest<AdvocateBrandRepository>
	{

		[Fact]
		public async void ShouldReturnZeroWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetIdleCountByBrand(Guid.Empty);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async void ShouldIncludeEntriesOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetAdvocateBrand(brandId, false),
				// exclude
				GetAdvocateBrand(Guid.NewGuid(), false),
				GetAdvocateBrand(Guid.NewGuid(), false),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetIdleCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyNotEnabledEntries()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetAdvocateBrand(brandId, false),
				// exclude
				GetAdvocateBrand(brandId, true),
				GetAdvocateBrand(brandId, true),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetIdleCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		private AdvocateBrand GetAdvocateBrand(Guid brandId, bool enabled)
		{
			Guid advocateId = Guid.NewGuid();
			return Builder<AdvocateBrand>.CreateNew()
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.BrandId, brandId)
				.With(x => x.Enabled, enabled)
				.With(x => x.User, Builder<User>.CreateNew()
								.With(x => x.Id, advocateId)
								.With(x => x.FirstName, "unit")
								.With(x => x.LastName, "test")
								.With(x => x.Email, advocateId.ToString() + "@unittest.com")
								.With(x => x.Phone, "12345467879")
								.With(x => x.Enabled, true)
								.Build())
				.Build();
		}
	}
}