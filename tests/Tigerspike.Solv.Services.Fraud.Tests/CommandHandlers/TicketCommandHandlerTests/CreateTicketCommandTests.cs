using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket;
using Tigerspike.Solv.Services.Fraud.Application.Events;
using Tigerspike.Solv.Services.Fraud.Domain;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.CommandsHandlers.TicketCommandHandlerTests
{
    public class CreateTicketCommandTests : BaseClass
    {
        [Fact]
        public async Task ShouldSucceedWhenInsertedIntoDatabase()
        {
            // Arrange
            var expectedTicket = GetMockTicket();
            var cmd = new CreateTicketCommand(Guid.Parse(expectedTicket.BrandId), Guid.Parse(expectedTicket.TicketId), (int)expectedTicket.Level,
                (int)expectedTicket.Status, Guid.Parse(expectedTicket.CustomerId), expectedTicket.AssignedTo, expectedTicket.BrandName, expectedTicket.Metadata, expectedTicket.CustomerDetail.FirstName, expectedTicket.CustomerDetail.LastName, expectedTicket.CustomerDetail.Email, expectedTicket.Question, string.Empty);

            // Act
            await TicketCommandHandler.Handle(cmd, CancellationToken.None);

            // Assert
            MockTicketRepository.Verify(m =>
                    m.AddOrUpdateTicket(It.Is<Ticket>(
                        actualApp => actualApp.AssignedTo == expectedTicket.AssignedTo
                                     && actualApp.TicketId == expectedTicket.TicketId
                                     && actualApp.CustomerId == expectedTicket.CustomerId
                                     && actualApp.BrandId == expectedTicket.BrandId
                                     && actualApp.BrandName == expectedTicket.BrandName))
                , Times.Once);

            MockMediator.Verify(
                m => m.RaiseEvent(It.IsAny<TicketCreatedEvent>()),
                Times.Once);
        }
    }
}