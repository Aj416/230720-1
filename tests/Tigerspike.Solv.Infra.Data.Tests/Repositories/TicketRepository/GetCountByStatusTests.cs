using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Repositories;
using Xunit;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories.TicketRepositoryTests
{
	public class GetCountByStatusTests : BaseRepositoryTest<TicketRepository>
	{

		[Fact]
		public async void ShouldReturnEmptyResultWhenThereIsNoData()
		{
			// Act
			var result = await SystemUnderTest.GetCountByStatusForAll();

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyFromSpecifiedBrand()
		{
			// Arrange
			var brandId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.New, brandId: brandId),
				// exclude
				GetTicket(TicketStatusEnum.New),
				GetTicket(TicketStatusEnum.New),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCountByStatusForBrand(brandId);

			// Assert
			Assert.Equal(new Dictionary<TicketStatusEnum, int> {
				[TicketStatusEnum.New] = 1,
			}, result);
		}

		[Fact]
		public async void ShouldIncludeTicketsOnlyForSpecifiedAdvocate()
		{
			// Arrange
			var advocateId = Guid.NewGuid();
			SystemUnderTest.Insert(new [] { 
				// include
				GetTicket(TicketStatusEnum.New, advocateId: advocateId),
				// exclude
				GetTicket(TicketStatusEnum.New),
				GetTicket(TicketStatusEnum.New),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCountByStatusForAdvocate(advocateId);

			// Assert
			Assert.Equal(new Dictionary<TicketStatusEnum, int> {
				[TicketStatusEnum.New] = 1,
			}, result);
		}

		[Fact]
		public async void ShouldIgnorePracticeTickets()
		{
			// Arrange
			SystemUnderTest.Insert(new[] {
				GetTicket(TicketStatusEnum.New, isPractice: true),
				GetTicket(TicketStatusEnum.Assigned, isPractice: true),
				GetTicket(TicketStatusEnum.Assigned),
				GetTicket(TicketStatusEnum.Closed, isPractice: true),
				GetTicket(TicketStatusEnum.Closed),
				GetTicket(TicketStatusEnum.Closed),
				GetTicket(TicketStatusEnum.Reserved, isPractice: true),
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Solved, isPractice: true),
				GetTicket(TicketStatusEnum.Solved),
				GetTicket(TicketStatusEnum.Solved),
				GetTicket(TicketStatusEnum.Solved),
				GetTicket(TicketStatusEnum.Solved),
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCountByStatusForAll();

			// Assert
			Assert.Equal(new Dictionary<TicketStatusEnum, int>
			{
				[TicketStatusEnum.Assigned] = 1,
				[TicketStatusEnum.Closed] = 2,
				[TicketStatusEnum.Reserved] = 3,
				[TicketStatusEnum.Solved] = 4,
			}, result);
		}

		[Fact]
		public async void ShouldGroupTicketsByStatus()
		{
			// Arrange
			SystemUnderTest.Insert(new [] { 
				GetTicket(TicketStatusEnum.New),
				GetTicket(TicketStatusEnum.Assigned),	
				GetTicket(TicketStatusEnum.Assigned),		
				GetTicket(TicketStatusEnum.Closed),	
				GetTicket(TicketStatusEnum.Closed),	
				GetTicket(TicketStatusEnum.Closed),	
				GetTicket(TicketStatusEnum.Reserved),	
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Reserved),
				GetTicket(TicketStatusEnum.Solved),
				GetTicket(TicketStatusEnum.Solved),	
				GetTicket(TicketStatusEnum.Solved),	
				GetTicket(TicketStatusEnum.Solved),	
				GetTicket(TicketStatusEnum.Solved),			
			});
			await SystemUnderTest.SaveChangesAsync();

			// Act
			var result = await SystemUnderTest.GetCountByStatusForAll();

			// Assert
			Assert.Equal(new Dictionary<TicketStatusEnum, int> {
				[TicketStatusEnum.New] = 1,
				[TicketStatusEnum.Assigned] = 2,
				[TicketStatusEnum.Closed] = 3,
				[TicketStatusEnum.Reserved] = 4,
				[TicketStatusEnum.Solved] = 5,
			}, result);
		}

		private Ticket GetTicket(TicketStatusEnum status, Guid? brandId = null, Guid? advocateId = null, bool? isPractice = false)
		{
			return Builder<Ticket>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.BrandId, brandId ?? Guid.NewGuid())
				.With(x => x.AdvocateId, advocateId ?? Guid.NewGuid())
				.With(x => x.Status, status)
				.With(x => x.IsPractice, isPractice ?? false)
				.Build();
		}
	}
}