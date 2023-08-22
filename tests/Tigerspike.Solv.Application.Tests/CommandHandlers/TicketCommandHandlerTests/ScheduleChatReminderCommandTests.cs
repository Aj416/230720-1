using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Bus.Scheduler;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
	public class ScheduleChatReminderCommandTests : BaseCommandHandlerTests<TicketCommandHandler>
    {

        public ScheduleChatReminderCommandTests()
        {
            Mocker.GetMock<IOptions<EmailTemplatesOptions>>()
                .Setup(s => s.Value)
                .Returns(new EmailTemplatesOptions
                {
					ChatReminderDelaySeconds = 10,
                    ChatUrl = "https://local/chat/{0}/{1}"
                });
        }

        [Fact]
        public async Task ShouldCancelPreviousSchedulesThenScheduleNewSend()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var scheduleCmd = new ScheduleChatReminderCommand(ticketId);
            var token = "ticketToken";
            var ticketData = (
				advocateFirstName: "SolverFirstName",
				brandThumbnail: "https://local/assets/brand.png",
				brandName: "BrandName",
				customerId: Guid.NewGuid(),
				customerEmail: "customer@mailinator.com",
				question: "This is test question"
            );

            var sendCmd = new SendChatReminderCommand(ticketId);

			Mocker.GetMock<ITicketRepository>()
                .Setup(x => x.GetFirstOrDefaultAsync<(string, string, string, Guid, string, string)>(
                    It.IsAny<Expression<Func<Ticket, (string, string, string, Guid, string, string)>>>(),
                    It.IsAny<Expression<Func<Ticket, bool>>>(),
                    null, null, true, It.IsAny<bool>())
                )
                .Returns(Task.FromResult(ticketData));

            Mocker.GetMock<IJwtService>()
                .Setup(x => x.CreateTokenForTicket(ticketId, ticketData.customerId))
                .Returns(new JwtModel(token, 1));

            // make sure that scheduler service methods are called in the following sequence
            var seq = new MockSequence();

            // first cancel previoues schedules
			Mocker.GetMock<ISchedulerService>(MockBehavior.Strict)
                .InSequence(seq)
                .Setup(x => x.CancelScheduledJob(It.IsAny<SendChatReminderCommand>()))
                .Returns(Task.CompletedTask);

            // then schedule new job
			Mocker.GetMock<ISchedulerService>(MockBehavior.Strict)
				.InSequence(seq)
				.Setup(x => x.ScheduleJob(It.IsAny<SendChatReminderCommand>(), It.IsAny<DateTime>()))
				.Returns(Task.CompletedTask);

            // Act
            await SystemUnderTest.Handle(scheduleCmd, CancellationToken.None);

            // Assert
            Mocker.GetMock<ISchedulerService>().Verify(x => x.CancelScheduledJob(It.Is<SendChatReminderCommand>(x => x.TicketId == ticketId)), Times.Once);
			Mocker.GetMock<ISchedulerService>().Verify(x => x.ScheduleJob(It.Is<SendChatReminderCommand>(x => x.TicketId == ticketId), It.IsAny<DateTime>()), Times.Once);
        }


    }
}
