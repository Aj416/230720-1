using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAverageCsatTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAverageCsat();

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
			var result = await SystemUnderTest.GetAverageCsat(brandId: brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyForSpecifiedAdvocateAndBrand()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1, advocateId: advocateId, brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 5, advocateId: advocateId, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 3, advocateId: advocateId, brandId: Guid.NewGuid()),
				GetTicket(TicketStatusEnum.Closed, 2),				
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageCsat(advocateId: advocateId, brandId: brandId);

			// Assert
			Assert.Equal(3, result);
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
			var result = await SystemUnderTest.GetAverageCsat();

			// Assert
			Assert.Equal(50, result);
		}

		[Fact]
		public async void ShouldIgnoreNullCsatTickets()
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
			var result = await SystemUnderTest.GetAverageCsat();

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
			var result = await SystemUnderTest.GetAverageCsat();

			// Assert
			Assert.Equal(2.25m, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, int? csat, Guid? brandId = null, Guid? advocateId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, advocateId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.Csat, csat)
				.Build();
		}
	}
}