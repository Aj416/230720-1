using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAverageTimeToCompleteTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete();

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			var diffrentTimestamp = new DateTime(2015, 01, 01, 15, 0, 20);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, startTimestamp, stopTimestamp, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Closed, startTimestamp, diffrentTimestamp),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete(brandId: brandId);

			// Assert
			Assert.Equal(15, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			var diffrentTimestamp = new DateTime(2015, 01, 01, 15, 0, 20);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, startTimestamp, stopTimestamp),
				// exclude
				GetTicket(TicketStatusEnum.New, startTimestamp, diffrentTimestamp),
				GetTicket(TicketStatusEnum.Assigned, startTimestamp, diffrentTimestamp),
				GetTicket(TicketStatusEnum.Reserved, startTimestamp, diffrentTimestamp),	
				GetTicket(TicketStatusEnum.Solved, startTimestamp, diffrentTimestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete();

			// Assert
			Assert.Equal(15, result);
		}		

		[Fact]
		public async void ShouldProperlyHandleNullDates()
		{
			// Arrange
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, startTimestamp, stopTimestamp),
				// exclude
				GetTicket(TicketStatusEnum.Closed, null, stopTimestamp),
				GetTicket(TicketStatusEnum.Closed, startTimestamp, null),
				GetTicket(TicketStatusEnum.Closed, null, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete();

			// Assert
			Assert.Equal(15, result);
		}

		[Fact]
		public async void ShouldProperlyCalculateAverageForEligibleTickets()
		{
			// Arrange
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 10)),
				GetTicket(TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 20)),
				GetTicket(TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 30)),
				GetTicket(TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 35)),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete();

			// Assert
			Assert.Equal(23.75, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, DateTime? firstAssignedDate, DateTime? closedDate, Guid? brandId = null)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.FirstAssignedDate, firstAssignedDate)
				.With(x => x.ClosedDate, closedDate)
				.Build();
		}
	}
}