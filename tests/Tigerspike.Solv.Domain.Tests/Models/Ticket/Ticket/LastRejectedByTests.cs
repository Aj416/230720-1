using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class LastRejectedByTests
	{
		[Fact]
		public void ShouldReturnNullWhenTicketWasNeverRejected()
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
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastRejectedBy;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnAdvocatesNameWhenTicketWasRejectedJustOnce()
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
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))					
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastRejectedBy;

			// Assert
			Assert.Equal("John Malkovic", result);
		}

		[Fact]
		public void ShouldReturnLastRejectedByWhenTicketWasRejectedMultipleTimes()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var acceptedDate = createdDate.AddSeconds(60);
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
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastRejectedBy;

			// Assert
			Assert.Equal("John Malkovic", result);
		}

		[Fact]
		public void ShouldReturnLastRejectedByWhenTicketWasEscalatedAfterLastRejection()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var acceptedDate = createdDate.AddSeconds(60);
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
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.With(x => x.Advocate, GetAdvocate("John", "Malkovic"))
					.Build(),
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastRejectedBy;

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