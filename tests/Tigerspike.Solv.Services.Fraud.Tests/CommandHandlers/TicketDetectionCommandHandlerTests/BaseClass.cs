using System;
using System.Collections.Generic;
using AutoMapper;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Services.Fraud.Application.Commands.TicketDetection;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Fraud.Tests.CommandsHandlers.TicketDetectionCommandHandlerTests
{
    public class BaseClass
    {
        protected readonly TicketDetectionCommandHandler TicketDetectionCommandHandler;
        protected readonly Mock<ITicketDetectionRepository> MockTicketDetectionRepository;
        protected readonly Mock<ITicketRepository> MockTicketRepository;
        protected readonly Mock<IRuleRepository> MockRuleRepository;
        protected readonly Mock<IMediatorHandler> MockMediator;
        protected readonly Mock<IMapper> MockMapper;

        protected BaseClass()
        {
            MockTicketDetectionRepository = new Mock<ITicketDetectionRepository>();
            MockRuleRepository = new Mock<IRuleRepository>();
            MockTicketRepository = new Mock<ITicketRepository>();
            MockMediator = new Mock<IMediatorHandler>();
            MockMapper = new Mock<IMapper>();

            TicketDetectionCommandHandler = new TicketDetectionCommandHandler(
                MockTicketDetectionRepository.Object,
                MockTicketRepository.Object,
                MockRuleRepository.Object,
                MockMediator.Object,
                MockMapper.Object);
        }

        protected static TicketDetection GetMockTicketDetectionWithAppliedRules()
        {
            var rules = new List<Guid>(){
                Builder<Guid>.CreateNew()
                .Build(),
                Builder<Guid>.CreateNew()
                .Build()
            };

            return Builder<TicketDetection>.CreateNew()
                .WithFactory(() => new TicketDetection(Guid.NewGuid(), 0, rules))
                .Build();
        }

        protected static TicketDetection GetMockTicketDetectionWithoutAppliedRules()
        {
            var rules = new List<Guid>();

            return Builder<TicketDetection>.CreateNew()
                .WithFactory(() => new TicketDetection(Guid.NewGuid(), 0, rules))
                .Build();
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