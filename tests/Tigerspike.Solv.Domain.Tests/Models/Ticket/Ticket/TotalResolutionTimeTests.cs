using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class TotalResolutionTimeTests
	{
		[Fact]
		public void ShouldReturnTotalResolutionTimeWhenTicketWasClosed()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var totalResolutionTime = 3;
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddMinutes(1))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddMinutes(2))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddMinutes(3))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddMinutes(4))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.CreatedDate, createdDate)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.TotalResolutionTime;

			// Assert
			Assert.Equal(totalResolutionTime, result);
		}

		[Fact]
		public void ShouldReturnNullWhenTicketIsNotClosed()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var totalResolutionTime = new TimeSpan(0, 0, 30);
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
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.CreatedDate, createdDate)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.TotalResolutionTime;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnNullWhenTicketIsNotSolved()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var totalResolutionTime = new TimeSpan(0, 0, 30);
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
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.CreatedDate, createdDate)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.TotalResolutionTime;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnTotalResolutionTimeWhenTicketIsReopenedDuringLifeteime()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var totalResolutionTime = 5;
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Reserved)
					.With(x => x.CreatedDate, createdDate.AddMinutes(1))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddMinutes(2))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddMinutes(3))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddMinutes(4))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddMinutes(5))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddMinutes(6))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.CreatedDate, createdDate)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.TotalResolutionTime;

			// Assert
			Assert.Equal(totalResolutionTime, result);
		}
	}
}