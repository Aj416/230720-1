using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetCriticalTicketsCountForBrandTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnZeroWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(Guid.Empty);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyInStatusNew()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = DateTime.UtcNow;
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(brandId, TicketStatusEnum.New, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				// exclude
				GetTicket(brandId, TicketStatusEnum.Assigned, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.Closed, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.Reserved, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.Solved, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsExceedingRejectCriticalCount()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = DateTime.UtcNow;
			SystemUnderTest.Insert(new [] { 
				// exclude
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.New, 1, 0, timestamp),
				// include
				GetTicket(brandId, TicketStatusEnum.New, 2, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.New, 3, 0, timestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(brandId);

			// Assert
			Assert.Equal(2, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsExceedingAbandonCriticalCount()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = DateTime.UtcNow;
			SystemUnderTest.Insert(new [] { 
				// exclude
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.New, 0, 1, timestamp),
				// include
				GetTicket(brandId, TicketStatusEnum.New, 0, 2, timestamp),
				GetTicket(brandId, TicketStatusEnum.New, 0, 3, timestamp),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(brandId);

			// Assert
			Assert.Equal(2, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsExceedingCriticalTimeThreshold()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = DateTime.UtcNow;
			Mocker.GetMock<ITimestampService>()
				.Setup(x => x.GetUtcTimestamp())
				.Returns(timestamp);

			SystemUnderTest.Insert(new [] { 
				//include
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp - Ticket.CriticalTicketTimeThreshold),
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp - Ticket.CriticalTicketTimeThreshold - new TimeSpan(0, 0, 1)),
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp - Ticket.CriticalTicketTimeThreshold - new TimeSpan(1, 0, 0)),
				// exclude
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp),
				GetTicket(brandId, TicketStatusEnum.New, 0, 0, timestamp + new TimeSpan(1, 0, 0)),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(brandId);

			// Assert
			Assert.Equal(3, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var timestamp = DateTime.UtcNow;
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(brandId, TicketStatusEnum.New, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				// exclude
				GetTicket(Guid.NewGuid(), TicketStatusEnum.New, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),
				GetTicket(Guid.NewGuid(), TicketStatusEnum.New, Ticket.REJECT_CRTICIAL_COUNT, 0, timestamp),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCriticalCountForBrand(brandId);

			// Assert
			Assert.Equal(1, result);
		}

		private Ticket GetTicket(Guid brandId, TicketStatusEnum status, int rejectionCount, int abandonedCount, DateTime timestamp)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId)
				.With(x => x.Status, status)
				.With(x => x.RejectionCount, rejectionCount)
				.With(x => x.AbandonedCount, abandonedCount)
				.With(x => x.ModifiedDate, timestamp)
				.Build();
		}
	}
}