using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class ReopenedCountTests
	{
		[Fact]
		public void ShouldReturnZeroWhenTicketWasNeverSolved()
		{
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
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Assigned)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public void ShouldReturnZeroWhenTicketWasNeverClosed()
		{
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
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Solved)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(0, result);
		}		

		[Fact]
		public void ShouldReturnZeroWhenTicketWasNeverReopened()
		{
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
					.Build(),										
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Closed)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public void ShouldReturnTwoWhenTicketWasTwiceReopenedAndClosed()
		{
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
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(70))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddSeconds(80))
					.Build(),
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Closed)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(2, result);
		}

		[Fact]
		public void ShouldReturnZeroWhenTicketWasTwiceReopenedButNotYetClosed()
		{
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
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(40))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(50))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(60))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, createdDate.AddSeconds(70))
					.Build(),
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Solved)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public void ShouldReturnZeroWhenTicketWasNeverReopenedAndClosedBySystemCutOff()
		{
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
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
			};

			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Closed)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ReopenedCount;

			// Assert
			Assert.Equal(0, result);
		}

	}
}