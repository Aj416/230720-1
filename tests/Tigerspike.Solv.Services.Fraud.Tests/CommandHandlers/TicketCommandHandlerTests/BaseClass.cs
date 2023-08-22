using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Tests.CommandsHandlers.TicketCommandHandlerTests
{
    public class BaseClass
    {
        protected readonly TicketCommandHandler TicketCommandHandler;
        protected readonly Mock<ITicketRepository> MockTicketRepository;
        protected readonly Mock<IMediatorHandler> MockMediator;
        protected readonly Mock<ILogger<TicketCommandHandler>> MockLogger;

        protected BaseClass()
        {
            MockTicketRepository = new Mock<ITicketRepository>();
            MockMediator = new Mock<IMediatorHandler>();
            MockLogger = new Mock<ILogger<TicketCommandHandler>>();

            TicketCommandHandler = new TicketCommandHandler(
                MockTicketRepository.Object,
                MockMediator.Object,
                MockLogger.Object);
        }

        protected static Ticket GetMockTicket() => Builder<Ticket>.CreateNew()
                .WithFactory(() => new Ticket(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "BrandName", 1, 0,
                "AdvocateName", new Dictionary<string, string>(), GetMockCustomerDetail(), "Customer-Question"))
                .Build();

        protected static Customer GetMockCustomerDetail() => Builder<Customer>.CreateNew()
                .WithFactory(() => new Customer("Customer-FirstName", "Customer-LastName", "Customer-Email"))
                .Build();
    }
}