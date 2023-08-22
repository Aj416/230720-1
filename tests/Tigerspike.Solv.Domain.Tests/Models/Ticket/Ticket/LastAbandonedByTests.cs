using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class LastAbandonedByTests
	{
		[Fact]
		public void ShouldReturnNullWhenTicketWasNeverAbandoned()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(10))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastAbandonedBy;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnAdvocatesNameWhenTicketWasAbandonedJustOnce()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
			};
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(10))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))					
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))					
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedBy;

			// Assert
			Assert.Equal("John Malkovic", result);
		}

		[Fact]
		public void ShouldReturnLastAdvocatesNameWhenTicketWasAbandonedMultipleTimes()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))
					.Build(),
			};
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(10))
					.With(x => x.Advocate, GetAdvocate("Steve", "Jobs"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.With(x => x.Advocate, GetAdvocate("Steve", "Jobs"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))					
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, createdDate.AddSeconds(70))					
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedBy;

			// Assert
			Assert.Equal("John Malkovic", result);
		}

		[Fact]
		public void ShouldReturnLastAdvocatesNameWhenTicketWasEscalatedAfterLastAbandoned()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))
					.Build(),
			};

			var history = new List<TicketStatusHistory>
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(10))
					.With(x => x.Advocate, GetAdvocate("Steve", "Jobs"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.With(x => x.Advocate, GetAdvocate("Steve", "Jobs"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedBy;

			// Assert
			Assert.Equal("John Malkovic", result);
		}		

		private Advocate GetAdvocate(string firstName, string lastName)
		{
			var user = Builder<User>.CreateNew()
				.With(x => x.FirstName, firstName)
				.With(x => x.LastName, lastName)
				.Build();

			return Builder<Advocate>.CreateNew()
				.With(x => x.User, user)
				.Build();
		}
	}
}