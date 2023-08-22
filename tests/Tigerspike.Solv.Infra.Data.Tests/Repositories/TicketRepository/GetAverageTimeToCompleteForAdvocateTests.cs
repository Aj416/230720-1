using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAverageTimeToCompleteForAdvocateTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var advocateId = Guid.NewGuid();
			var result = await SystemUnderTest.GetAverageTimeToComplete(advocateId: advocateId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedAdvocate()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			var diffrentTimestamp = new DateTime(2015, 01, 01, 15, 0, 20);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, stopTimestamp),
				// exclude
				GetTicket(Guid.NewGuid(), TicketStatusEnum.Closed, startTimestamp, diffrentTimestamp),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete(advocateId: advocateId);

			// Assert
			Assert.Equal(15, result);
		}

		[Fact]
		public async void ShouldIncludeOnlyClosedTickets()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			var diffrentTimestamp = new DateTime(2015, 01, 01, 15, 0, 20);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, stopTimestamp),
				// exclude
				GetTicket(advocateId, TicketStatusEnum.New, startTimestamp, diffrentTimestamp),
				GetTicket(advocateId, TicketStatusEnum.Assigned, startTimestamp, diffrentTimestamp),
				GetTicket(advocateId, TicketStatusEnum.Reserved, startTimestamp, diffrentTimestamp),	
				GetTicket(advocateId, TicketStatusEnum.Solved, startTimestamp, diffrentTimestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete(advocateId: advocateId);

			// Assert
			Assert.Equal(15, result);
		}		

		[Fact]
		public async void ShouldProperlyHandleNullDates()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, stopTimestamp),
				// exclude
				GetTicket(advocateId, TicketStatusEnum.Closed, null, stopTimestamp),
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, null),
				GetTicket(advocateId, TicketStatusEnum.Closed, null, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete(advocateId: advocateId);

			// Assert
			Assert.Equal(15, result);
		}

		[Fact]
		public async void ShouldProperlyCalculateAverageForEligibleTickets()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 10)),
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 20)),
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 30)),
				GetTicket(advocateId, TicketStatusEnum.Closed, startTimestamp, startTimestamp + new TimeSpan(0, 0, 35)),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToComplete(advocateId: advocateId);

			// Assert
			Assert.Equal(23.75, result);
		}

		private Ticket GetTicket(Guid advocateId, TicketStatusEnum status, DateTime? assignedDate, DateTime? closedDate)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, Guid.NewGuid())
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.Status, status)
				.With(x => x.AssignedDate, assignedDate)
				.With(x => x.ClosedDate, closedDate)
				.Build();
		}
	}
}