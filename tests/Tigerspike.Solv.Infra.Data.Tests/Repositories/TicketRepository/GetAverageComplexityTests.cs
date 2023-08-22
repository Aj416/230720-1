using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAverageComplexityTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAverageComplexityForAll();

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 2),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageComplexityForBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 50),
				// exclude
				GetTicket(TicketStatusEnum.New, 2),
				GetTicket(TicketStatusEnum.Assigned, 2),	
				GetTicket(TicketStatusEnum.Reserved, 2),	
				GetTicket(TicketStatusEnum.Solved, 2),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageComplexityForAll();

			// Assert
			Assert.Equal(50, result);
		}

		[Fact]
		public async void ShouldIgnoreNullComplexityTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1),
				GetTicket(TicketStatusEnum.Closed, 2),
				GetTicket(TicketStatusEnum.Closed, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageComplexityForAll();

			// Assert
			Assert.Equal(1.5m, result);
		}

		[Fact]
		public async void ShouldProperlyCalculateAverageForEligibleTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1),
				GetTicket(TicketStatusEnum.Closed, 1),
				GetTicket(TicketStatusEnum.Closed, 2),
				GetTicket(TicketStatusEnum.Closed, 5),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageComplexityForAll();

			// Assert
			Assert.Equal(2.25m, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, int? complexity, Guid? brandId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.Complexity, complexity)
				.Build();
		}
	}
}