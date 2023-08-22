using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class FlatLastRejectionReasonsTests
	{
		[Fact]
		public void ShouldReturnNullWhenThereWasNoRejections()
		{
			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.RejectionHistory, new List<TicketRejectionHistory>())
				.Build();

			// Act
			var result = ticket.LastRejectionReasonsNames;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnNullWhenThereNoReasonsInRejection()
		{
			// Arrange			
			var firstRejection = new DateTime(2010, 2, 2);
			var rejections = new [] { GetRejection(firstRejection) };
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.RejectionHistory, rejections.ToList())
				.Build();			

			// Act
			var result = ticket.LastRejectionReasonsNames;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnReasonWhenThereWasOneReasonInOneRejection()
		{
			// Arrange
			var firstRejection = new DateTime(2010, 2, 2);
			var rejections = new[] { GetRejection(firstRejection, "test") };
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.RejectionHistory, rejections.ToList())
				.Build();

			// Act
			var result = ticket.LastRejectionReasonsNames;

			// Assert
			Assert.Equal("test", result);
		}

		[Fact]
		public void ShouldReturnSeparatedReasonsWhenThereWasMultipleReasonsInOneRejection()
		{
			// Arrange
			var firstRejection = new DateTime(2010, 2, 2);
			var rejections = new[] { GetRejection(firstRejection, "test1", "test2") };
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.RejectionHistory, rejections.ToList())
				.Build();

			// Act
			var result = ticket.LastRejectionReasonsNames;

			// Assert
			Assert.Equal("test1|test2", result);
		}

		[Fact]
		public void ShouldReturnSeparatedReasonsWhenThereWasMultipleReasonsInManyRejections()
		{
			// Arrange
			var firstRejection = new DateTime(2010, 2, 2);
			var rejections = new[] {
				GetRejection(firstRejection.AddDays(1), "test3", "test4"),
				GetRejection(firstRejection, "test1", "test2"),				
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.RejectionHistory, rejections.ToList())
				.Build();

			// Act
			var result = ticket.LastRejectionReasonsNames;

			// Assert
			Assert.Equal("test3|test4", result);
		}

		private TicketRejectionHistory GetRejection(DateTime timestamp, params string[] reasonsTexts)
		{
			var reasons = new List<TicketRejectionReason>();
			foreach (var reason in reasonsTexts)
			{
				var rejectionReasonEntity = Builder<RejectionReason>
					.CreateNew()
					.With(x => x.Name, reason)
					.Build();

				var ticketRejectionReasonEntity = Builder<TicketRejectionReason>.CreateNew()
					.With(x => x.RejectionReason, rejectionReasonEntity)
					.Build();				

				reasons.Add(ticketRejectionReasonEntity);
			}

			return Builder<TicketRejectionHistory>
					.CreateNew()
					.With(x => x.Reasons, reasons)
					.With(x => x.CreatedDate, timestamp)
					.Build();
		}

	}
}