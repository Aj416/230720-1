using FizzWare.NBuilder;
using System;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetAverageTimeToRespondTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnNullWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetAverageTimeToRespondForAll();

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void ShouldIgnorePracticeTickets()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			var startTimestamp = new DateTime(2015, 01, 01, 15, 0, 0);
			var stopTimestamp = new DateTime(2015, 01, 01, 15, 0, 15);
			var diffrentTimestamp = new DateTime(2015, 01, 01, 15, 0, 20);
			SystemUnderTest.Insert(new[] { 
				// include
				GetTicket(startTimestamp, stopTimestamp),
				// exclude
				GetTicket(startTimestamp, diffrentTimestamp, isPractice: true),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToRespondForAll();

			// Assert
			Assert.Equal(15, result);
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
				GetTicket(startTimestamp, stopTimestamp, brandId: brandId),
				// exclude
				GetTicket(startTimestamp, diffrentTimestamp),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToRespondForBrand(brandId);

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
				GetTicket(startTimestamp, stopTimestamp),
				// exclude
				GetTicket(startTimestamp, null),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToRespondForAll();

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
				GetTicket(startTimestamp, startTimestamp + new TimeSpan(0, 0, 10)),
				GetTicket(startTimestamp, startTimestamp + new TimeSpan(0, 0, 20)),
				GetTicket(startTimestamp, startTimestamp + new TimeSpan(0, 0, 30)),
				GetTicket(startTimestamp, startTimestamp + new TimeSpan(0, 0, 35)),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetAverageTimeToRespondForAll();

			// Assert
			Assert.Equal(23.75, result);
		}

		private Ticket GetTicket(DateTime createdDate, DateTime? firstMessageDate, Guid? brandId = null, bool? isPractice = false)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.CreatedDate, createdDate)
				.With(x => x.FirstMessageDate, firstMessageDate)
				.With(x => x.IsPractice, isPractice ?? false)
				.Build();
		}
	}
}