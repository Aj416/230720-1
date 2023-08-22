using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;
namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
	public class UpdateTicketMessageStatisticsCommandHandlerTest : BaseCommandHandlerTests<TicketCommandHandler>
    {
        protected Ticket Ticket { get; }

        public UpdateTicketMessageStatisticsCommandHandlerTest()
        {
			Ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.SolverResponseCount, 0)
				.With(x => x.CustomerMessageCount, 0)
				.With(x => x.SolverMessageCount, 0)
				.With(x => x.SolverMaxResponseTimeInSeconds, null)
                .With(x => x.SolverTotalResponseTimeInSeconds, null)
				.With(x => x.LastCustomerMessageDate, null)
				.With(x => x.LastAdvocateMessageDate, null)
				.With(x => x.CurrentCustomerQueryDate, null)				
				.Build();

            Mocker.GetMock<ITicketRepository>()
                .Setup(x => x.FindAsync(Ticket.Id))
                .ReturnsAsync(Ticket);
        }

        [Fact]
        public async Task TicketWithOnlyCustomerQuestionShouldHaveNoResponseStatistics()
        {
            // Arrange
            var messages = new [] {
                CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
            };
            
            // Act
            foreach (var msg in messages)
            {
                await SystemUnderTest.Handle(msg, CancellationToken.None);
            }

            // Assert
            Assert.Equal(1, Ticket.CustomerMessageCount);
			Assert.Equal(0, Ticket.SolverMessageCount);
			Assert.Null(Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Null(Ticket.AverageSolverResponseTime);
        }

		[Fact]
		public async Task TicketWithOneQuestionsOneAnswerShouldHaveEqualStatistics()
		{
			// Arrange
			var messages = new[] {
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 20), UserType.Advocate),
			};

			// Act
			foreach (var msg in messages)
			{
				await SystemUnderTest.Handle(msg, CancellationToken.None);
			}

			// Assert
			Assert.Equal(1, Ticket.CustomerMessageCount);
			Assert.Equal(1, Ticket.SolverMessageCount);
			Assert.Equal(20, Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Equal(20, Ticket.AverageSolverResponseTime);
		}

		[Fact]
		public async Task TicketWithRegularAndSuperSolverResponsesShouldHaveCorrectStatistics()
		{
			// Arrange
			var messages = new[] {
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 20), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 1, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 1, 20), UserType.SuperSolver),
			};

			// Act
			foreach (var msg in messages)
			{
				await SystemUnderTest.Handle(msg, CancellationToken.None);
			}

			// Assert
			Assert.Equal(2, Ticket.CustomerMessageCount);
			Assert.Equal(2, Ticket.SolverMessageCount);
			Assert.Equal(20, Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Equal(20, Ticket.AverageSolverResponseTime);
		}

		[Fact]
		public async Task TicketWithManyCustomerQuestionsAndOneSolverResponseShouldHaveStatisticsBasedOnFirstCustomerQuery()
		{
			// Arrange
			var messages = new[] {
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 2), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 4), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 5), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 20), UserType.Advocate),
			};

			// Act
			foreach (var msg in messages)
			{
				await SystemUnderTest.Handle(msg, CancellationToken.None);
			}

			// Assert
			Assert.Equal(4, Ticket.CustomerMessageCount);
			Assert.Equal(1, Ticket.SolverMessageCount);
			Assert.Equal(20, Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Equal(20, Ticket.AverageSolverResponseTime);
		}

		[Fact]
		public async Task TicketWithManySolverResponsesAndOneCustomerQueryShouldHaveStatisticsBasedOnFirstSolverResponse()
		{
			// Arrange
			var messages = new[] {
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 2), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 3), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 4), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 20), UserType.Advocate),
			};

			// Act
			foreach (var msg in messages)
			{
				await SystemUnderTest.Handle(msg, CancellationToken.None);
			}

			// Assert
			Assert.Equal(1, Ticket.CustomerMessageCount);
			Assert.Equal(4, Ticket.SolverMessageCount);
			Assert.Equal(2, Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Equal(2, Ticket.AverageSolverResponseTime);
		}

		[Fact]
		public async Task TicketWithManyQuestionResponseChunksShouldHaveCorrectStatistics()
		{
			// Arrange
			var messages = new[] {
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 0, 3), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 1, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 1, 13), UserType.Advocate),
				CreateMessage(new DateTime(2020, 1, 1, 12, 2, 0), UserType.Customer),
				CreateMessage(new DateTime(2020, 1, 1, 12, 2, 5), UserType.Advocate),
			};

			// Act
			foreach (var msg in messages)
			{
				await SystemUnderTest.Handle(msg, CancellationToken.None);
			}

			// Assert
			Assert.Equal(3, Ticket.CustomerMessageCount);
			Assert.Equal(3, Ticket.SolverMessageCount);
			Assert.Equal(13, Ticket.SolverMaxResponseTimeInSeconds);
			Assert.Equal(7, Ticket.AverageSolverResponseTime);
		}        


		private UpdateTicketMessageStatisticsCommand CreateMessage(DateTime timestamp, UserType userType) => new UpdateTicketMessageStatisticsCommand(Ticket.Id, timestamp, (int)userType);        
    }
}
