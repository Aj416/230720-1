using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetTotalPriceTests : BaseRepositoryTest<TicketRepository>
	{
		[Fact]
		public async void ShouldReturnZeroWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetTotalPriceForAll();

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 7.00m, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 7.00m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetTotalPriceForBrand(brandId);

			// Assert
			Assert.Equal(10.00m, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedPeriod()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var fromDate = new DateTime(2020, 1, 1);
			var toDate = new DateTime(2020, 1, 3);
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m, fromDate, brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,fromDate.AddHours(5), brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,fromDate.AddDays(1).AddHours(5), brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,toDate, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,fromDate.AddDays(-1), brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,fromDate.AddHours(-5), brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,toDate.AddHours(5), brandId: brandId),
				GetTicket(TicketStatusEnum.Closed, 7.00m, 3m,toDate.AddDays(1).AddHours(5), brandId: brandId),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetTotalPriceForBrand(brandId, fromDate, toDate);

			// Assert
			Assert.Equal(40.00m, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedAdvocate()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 7.00m, advocateId: advocateId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 7.00m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetTotalPriceForAdvocate(advocateId);

			// Assert without fees will be calculated
			Assert.Equal(7.00m, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 7.00m),
				// exclude
				GetTicket(TicketStatusEnum.New, 7.00m),
				GetTicket(TicketStatusEnum.Assigned, 7.00m),
				GetTicket(TicketStatusEnum.Reserved, 7.00m),
				GetTicket(TicketStatusEnum.Solved, 7.00m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetTotalPriceForAll();

			// Assert (7 + 3 = 10)
			Assert.Equal(10, result);
		}

		[Fact]
		public async void ShouldProperlySumEligibleTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 7.00m),
				GetTicket(TicketStatusEnum.Closed, 7.00m),
				GetTicket(TicketStatusEnum.Closed, 7.00m),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetTotalPriceForAll();

			// Assert
			Assert.Equal(30.0m, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, decimal price, decimal fee = 3.0m, DateTime? closedDate = null, Guid? brandId = null, Guid? advocateId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, advocateId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.Price, price)
				.With(x => x.Fee, fee)
				.With(x => x.ClosedDate, closedDate ?? DateTime.UtcNow.AddHours(-1))
				.Build();
		}
	}
}