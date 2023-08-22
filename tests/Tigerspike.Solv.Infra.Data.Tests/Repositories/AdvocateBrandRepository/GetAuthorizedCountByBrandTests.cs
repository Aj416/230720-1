using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.AdvocateBrandRepositoryTests
{
	public class GetAuthorizedCountByBrandTests : BaseRepositoryTest<AdvocateBrandRepository>
	{

		[Fact]
		public async void ShouldReturnZeroWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAuthorizedCountByBrand(Guid.Empty);

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
				GetAdvocateBrand(brandId, true, true),
				// exclude
				GetAdvocateBrand(Guid.NewGuid(), true, true),
				GetAdvocateBrand(Guid.NewGuid(), true, true),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAuthorizedCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyAuthorizedAndEnabledEntries()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetAdvocateBrand(brandId, true, true),
				// exclude
				GetAdvocateBrand(brandId, true, false),
				GetAdvocateBrand(brandId, false, true),		
				GetAdvocateBrand(brandId, false, false),		
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAuthorizedCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		private AdvocateBrand GetAdvocateBrand(Guid brandId, bool enabled, bool authorized)
		{
			Guid advocateId = Guid.NewGuid();

			return Builder<AdvocateBrand>.CreateNew()
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.BrandId, brandId)
				.With(x => x.Enabled, enabled)
				.With(x => x.Authorized, authorized)
				.With(x => x.User, Builder<User>.CreateNew()
								.With(x => x.Id, advocateId)
								.With(x => x.FirstName, "unit")
								.With(x => x.LastName, "test")
								.With(x => x.Email, advocateId.ToString() + "@unittest.com")
								.With(x => x.Phone, "12345467879")
								.With(x => x.Enabled, true)
								.Build()
				)
				.Build();
		}
	}
}