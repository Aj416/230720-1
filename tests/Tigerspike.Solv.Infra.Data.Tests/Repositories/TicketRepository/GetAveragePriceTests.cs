using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAveragePriceTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAveragePriceForAll();

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
				GetTicket(TicketStatusEnum.Closed, 1.00m, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 2.00m),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAveragePriceForBrand(brandId);

			// Assert
			Assert.Equal(1.00m, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedAdvocate()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1.00m, advocateId: advocateId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 2.00m),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAveragePriceForAdvocate(advocateId);

			// Assert
			Assert.Equal(0.7m, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 50.00m),
				// exclude
				GetTicket(TicketStatusEnum.New, 2.00m),
				GetTicket(TicketStatusEnum.Assigned, 2.00m),	
				GetTicket(TicketStatusEnum.Reserved, 2.00m),	
				GetTicket(TicketStatusEnum.Solved, 2.00m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAveragePriceForAll();

			// Assert
			Assert.Equal(50.00m, result);
		}

		[Fact]
		public async void ShouldProperlySumEligibleTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 1.00m),
				GetTicket(TicketStatusEnum.Closed, 1.23m),
				GetTicket(TicketStatusEnum.Closed, 1.0045m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAveragePriceForAll();

			// Assert
			Assert.Equal(3.2345m / 3.0m, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, decimal price, Guid? brandId = null, Guid? advocateId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, advocateId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.Price, (price * 0.7m))
                .With(x => x.Fee, (price * 0.3m))
                .Build();
		}
	}
}