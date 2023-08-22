using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.AdvocateBrandRepositoryTests
{
	public class GetBlockedCountByBrandTests : BaseRepositoryTest<AdvocateBrandRepository>
	{

		[Fact]
		public async void ShouldReturnZeroWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetBlockedCountByBrand(Guid.Empty);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async void ShouldIncludeEntriesOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var adv = GetUser(false);
			DbContext.Add(adv);
			DbContext.Add(GetUser(false));

			SystemUnderTest.Insert(new [] { 
				// include
				GetAdvocateBrand(brandId, adv.Id),
				// exclude
				GetAdvocateBrand(Guid.NewGuid(), adv.Id),
				GetAdvocateBrand(Guid.NewGuid(), adv.Id),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetBlockedCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyBlockedEntries()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var blocked = GetUser(false);
			var notBlocked1 = GetUser(true);
			var notBlocked2 = GetUser(true);
			DbContext.AddRange(blocked, notBlocked1, notBlocked2);

			SystemUnderTest.Insert(new [] { 
				// include
				GetAdvocateBrand(brandId, blocked.Id),
				// exclude
				GetAdvocateBrand(brandId, notBlocked1.Id),	
				GetAdvocateBrand(brandId, notBlocked2.Id),		
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetBlockedCountByBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		private AdvocateBrand GetAdvocateBrand(Guid brandId, Guid advocateId)
		{
			return Builder<AdvocateBrand>.CreateNew()
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.BrandId, brandId)
				.Build();
		}

		private User GetUser(bool enabled)
		{
			return Builder<User>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.Enabled, enabled)
				.With(x => x.Email, Guid.NewGuid().ToString())
				.Build();
		}
	}
}