using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetFirstAttemptSuccessRateTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetFirstAttemptSuccessRateForAll();

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
				GetTicket(TicketStatusEnum.Closed, 0, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, 0),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetFirstAttemptSuccessRateForBrand(brandId);

			// Assert
			Assert.Equal(100, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, 0),
				// exclude
				GetTicket(TicketStatusEnum.New, 0),
				GetTicket(TicketStatusEnum.Assigned, 0),	
				GetTicket(TicketStatusEnum.Reserved, 0),	
				GetTicket(TicketStatusEnum.Solved, 0),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetFirstAttemptSuccessRateForAll();

			// Assert
			Assert.Equal(100, result);
		}

		[Fact]
		public async void ShouldProperlyCalculateFirstAttemptSuccessRate()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				// first attempt
				GetTicket(TicketStatusEnum.Closed, 0),
				GetTicket(TicketStatusEnum.Closed, 0),
				// more than one attempt
				GetTicket(TicketStatusEnum.Closed, 1),
				GetTicket(TicketStatusEnum.Closed, 2),
				GetTicket(TicketStatusEnum.Closed, 3),
				GetTicket(TicketStatusEnum.Closed, 4),
				// not yet finished, excluded
				GetTicket(TicketStatusEnum.New, 0),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetFirstAttemptSuccessRateForAll();

			// Assert
			Assert.Equal(33, result); // 2 out of 6 = 33%
		}

		private Ticket GetTicket(TicketStatusEnum status, int abandonedCount, Guid? brandId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.AbandonedCount, abandonedCount)
				.Build();
		}
	}
}