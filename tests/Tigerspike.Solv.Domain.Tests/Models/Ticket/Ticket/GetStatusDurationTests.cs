using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class GetStatusDurationTests
	{
		private readonly Mock<ITimestampService> timestampServiceMock;
		public GetStatusDurationTests()
		{
			timestampServiceMock = new Mock<ITimestampService>();
			timestampServiceMock.Setup(x => x.GetUtcTimestamp()).Returns(new DateTime(2020, 01, 01, 15, 0, 0));
		}

		[Fact]
		public void ShouldReturnZeroWhenTicketWasNeverInDesiredStatus()
		{
			// Arrange
			var createdDate = new DateTime(2010, 1, 1);
			var history = new List<TicketStatusHistory> 
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
			};

			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.New)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.GetStatusDuration(TicketStatusEnum.Closed, timestampServiceMock.Object);

			// Assert
			Assert.Equal(0, result.TotalSeconds);
		}

		[Fact]
		public void ShouldReturnDifferenceBetweenNowAndCreationDateIfTicketWasEverOnlyInDesiredStatus()
		{
			// Arrange
			var createdDate = new DateTime(2020, 1, 1, 14, 45, 15);
			var history = new List<TicketStatusHistory>
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
			};

			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.New)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.GetStatusDuration(TicketStatusEnum.New, timestampServiceMock.Object);

			// Assert
			Assert.Equal(new TimeSpan(0, 14, 45).TotalSeconds, result.TotalSeconds);
		}

		[Fact]
		public void ShouldReturnAccumulatedDurationThatTicketWasInDesiredStatus()
		{
			// Arrange
			var createdDate = new DateTime(2020, 1, 1);
			var history = new List<TicketStatusHistory>
			{
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate)
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(30))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.New)
					.With(x => x.CreatedDate, createdDate.AddSeconds(45))
					.Build(),
				Builder<TicketStatusHistory>.CreateNew()
					.With(x => x.Status, TicketStatusEnum.Assigned)
					.With(x => x.CreatedDate, createdDate.AddSeconds(48))
					.Build(),					
			};

			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Status, TicketStatusEnum.Assigned)
				.With(x => x.StatusHistory, history)
				.Build();

			// Act
			var result = ticket.GetStatusDuration(TicketStatusEnum.New, timestampServiceMock.Object);

			// Assert
			Assert.Equal(33, result.TotalSeconds);
		}		



	}
}