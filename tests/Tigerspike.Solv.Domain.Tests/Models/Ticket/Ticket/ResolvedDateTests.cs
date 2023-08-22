using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class ResolvedDateTests
	{
		[Fact]
		public void ShouldReturnSolvedDateWhenTicketWasClosed()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var solvedDate = createdDate.AddSeconds(30);
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
					.With(x => x.CreatedDate, solvedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, solvedDate.AddSeconds(10))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ResolvedDate;

			// Assert
			Assert.Equal(solvedDate, result);
		}

		[Fact]
		public void ShouldReturnNullWhenTicketIsNotClosed()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var solvedDate = createdDate.AddSeconds(30);
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
					.With(x => x.CreatedDate, solvedDate)
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ResolvedDate;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnNullWhenTicketIsNotSolved()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var solvedDate = createdDate.AddSeconds(30);
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
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ResolvedDate;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnLastSolvedDateWhenTicketIsReopenedDuringLifeteime()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var solvedDate = createdDate.AddSeconds(50);
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
					.With(x => x.CreatedDate, solvedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, solvedDate.AddSeconds(10))
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.ResolvedDate;

			// Assert
			Assert.Equal(solvedDate, result);
		}
	}
}