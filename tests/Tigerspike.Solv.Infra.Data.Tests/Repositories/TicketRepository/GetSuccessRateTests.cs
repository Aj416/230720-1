using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetSuccessRateTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetSuccessRate();

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = new DateTime(2015, 01, 01);
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.Closed, timestamp, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.Assigned, timestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetSuccessRate(brandId: brandId);

			// Assert
			Assert.Equal(100, result);
		}

		[Fact]
		public async void ShouldProperlyCalculateSuccessRateForAdvocate()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			var otherAdvocateId = Guid.NewGuid();
			var timestamp = new DateTime(2015, 01, 01);

			var abandonedTicket = GetTicket(TicketStatusEnum.Closed, timestamp, advocateId: otherAdvocateId);
			abandonedTicket.StatusHistory = new List<TicketStatusHistory> {
				GetTicketHistoryItem(abandonedTicket.Id, advocateId, TicketStatusEnum.Assigned),
				GetTicketHistoryItem(abandonedTicket.Id, otherAdvocateId, TicketStatusEnum.Assigned),
				GetTicketHistoryItem(abandonedTicket.Id, otherAdvocateId, TicketStatusEnum.Solved),
				GetTicketHistoryItem(abandonedTicket.Id, otherAdvocateId, TicketStatusEnum.Closed)
			};

			SystemUnderTest.Insert(new[] { 
				// success
				GetTicket(TicketStatusEnum.Closed, timestamp, advocateId: advocateId),
				// abandoned
				abandonedTicket,
				// excluded
				GetTicket(TicketStatusEnum.New, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetSuccessRate(advocateId: advocateId);

			// Assert
			Assert.Equal(50, result); // 1 out of 2 = 50%
		}
		
		[Fact]
		public async void ShouldProperlyCalculateSuccessRate()
		{
			// Arrange
			var timestamp = new DateTime(2015, 01, 01);
			SystemUnderTest.Insert(new [] { 
				// success
				GetTicket(TicketStatusEnum.Closed, timestamp),
				GetTicket(TicketStatusEnum.Closed, timestamp),
				// not yet success
				GetTicket(TicketStatusEnum.Reserved, timestamp),
				GetTicket(TicketStatusEnum.Assigned, timestamp),
				GetTicket(TicketStatusEnum.New, timestamp),
				GetTicket(TicketStatusEnum.Solved, timestamp),
				// excluded
				GetTicket(TicketStatusEnum.New, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetSuccessRate();

			// Assert
			Assert.Equal(33, result); // 2 out of 6 = 33%
		}

		private Ticket GetTicket(TicketStatusEnum status, DateTime? firstAssignedDate, Guid? brandId = null, Guid? advocateId = null)
		{
			var ticketId = Guid.NewGuid();
			var advId = advocateId ?? Guid.NewGuid();
			var history = firstAssignedDate != null ? new List<TicketStatusHistory> { GetTicketHistoryItem(ticketId, advId, TicketStatusEnum.Assigned) } : null;
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, ticketId)
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, advId)
				.With(x => x.Status, status)
				.With(x => x.FirstAssignedDate, firstAssignedDate)
				.With(x => x.StatusHistory, history)
				.Build();
		}

		private TicketStatusHistory GetTicketHistoryItem(Guid ticketId, Guid advocateId, TicketStatusEnum status)
		{
			return Builder<TicketStatusHistory>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.TicketId, ticketId)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.Status, status)
				.Build();
		}
	}
}