using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class LastAcceptedDateTests
	{
		[Fact]
		public void ShouldReturnNullWhenTicketWasNeverAccepted()
		{
			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, new List<TicketStatusHistory>())
				.Build();

			// Act
			var result = ticket.LastAcceptedDate;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnAcceptanceDateWhenTicketWasAcceptedJustOnce()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var acceptedDate = createdDate.AddSeconds(20);
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
					.With(x => x.CreatedDate, acceptedDate)
					.Build()
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastAcceptedDate;

			// Assert
			Assert.Equal(acceptedDate, result);
		}

		[Fact]
		public void ShouldReturnLastAcceptanceDateWhenTicketWasAcceptedMultipleTimes()
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
					.With(x => x.CreatedDate, acceptedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, acceptedDate.AddSeconds(20))
					.Build(),
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastAcceptedDate;

			// Assert
			Assert.Equal(acceptedDate, result);
		}

		[Fact]
		public void ShouldReturnAcceptanceDateWhenRegardlessOfTheFactThatTicketWasReopened()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var acceptedDate = createdDate.AddSeconds(20);
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
					.With(x => x.CreatedDate, acceptedDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, acceptedDate.AddSeconds(10))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, acceptedDate.AddSeconds(20))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Solved)
					.With(x => x.CreatedDate, acceptedDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Closed)
					.With(x => x.CreatedDate, acceptedDate.AddSeconds(40))
					.Build(),					
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.LastAcceptedDate;

			// Assert
			Assert.Equal(acceptedDate, result);
		}		

	}
}