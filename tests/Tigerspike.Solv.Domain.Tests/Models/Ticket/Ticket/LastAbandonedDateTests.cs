using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class LastAbandonedDateTests
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
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.Build()
			};			
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastAbandonedDate;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnLastAbandonedDateWhenTicketWasAbandonedJustOnce()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonedDate = createdDate.AddSeconds(30);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, abandonedDate)
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
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, abandonedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, abandonedDate)
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedDate;

			// Assert
			Assert.Equal(abandonedDate, result);
		}

		[Fact]
		public void ShouldReturnLastAbandonedDateWhenTicketWasAbandonedMultipleTimes()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonedDate = createdDate.AddSeconds(60);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, abandonedDate)
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
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, abandonedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, abandonedDate)
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedDate;

			// Assert
			Assert.Equal(abandonedDate, result);
		}

		[Fact]
		public void ShouldReturnLastAbandonedDateWhenTicketWasEscalatedAfterLastAbandoned()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var abandonedDate = createdDate.AddSeconds(60);
			var abandonHistory = new List<TicketAbandonHistory>
			{
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketAbandonHistory>.CreateNew()
					.With(x => x.CreatedDate, abandonedDate)
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
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(20))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Escalated)
					.With(x => x.CreatedDate, abandonedDate)
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.With(x => x.AbandonHistory, abandonHistory)
				.Build();

			// Act
			var result = ticket.LastAbandonedDate;

			// Assert
			Assert.Equal(abandonedDate, result);
		}		
	}
}