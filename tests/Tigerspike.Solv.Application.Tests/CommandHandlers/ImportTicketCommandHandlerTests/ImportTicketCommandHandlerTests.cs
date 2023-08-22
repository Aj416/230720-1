using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMoqCore;
using FizzWare.NBuilder;
using MediatR;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers.Import;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers
{
    public class ImportTicketCommandHandlerTests
    {
        protected readonly AutoMoqer Mocker = new AutoMoqer();
        protected readonly Brand Brand = Brand.CreateDemoBrand("demo", string.Empty, string.Empty);
        protected ImportTicketCommandHandler SystemUnderTest => Mocker.Create<ImportTicketCommandHandler>();
        protected Ticket Ticket;

        public ImportTicketCommandHandlerTests()
        {
            Brand.SetTicketPrice(3.00m);

            Mocker.GetMock<IUnitOfWork>()
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            Mocker.GetMock<ITicketRepository>()
                .Setup(x => x.Insert(It.IsAny<Ticket>()))
                .Callback<Ticket>(x => Ticket = x); // store created ticket for verification purposes

            var brandRepoMock = Mocker.GetMock<IBrandRepository>();
            brandRepoMock
                .Setup(x => x.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Brand);
        }

        [Fact]
        public async Task WhenTicketIsHandledItsReferenceShouldBeAvailableForTesting()
        {
            var cmd = ImportTicketCommandBuilder.Build();

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.NotNull(id);
            Assert.NotEqual(Guid.Empty, id);
            Assert.NotNull(Ticket);
            Assert.Equal(id, Ticket.Id);
        }

        [Fact]
        public async Task WhenTicketIsHandledTicketImportedEventShouldBeRaised()
        {
            var cmd = ImportTicketCommandBuilder.Build();

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);

            Mocker.GetMock<IMediatorHandler>()
                .Verify(x => x.RaiseEvent(It.Is<TicketImportedEvent>(e => e.TicketId == id)), Times.Once);
        }

        [Fact]
        public async Task WhenNoAdvocateDetailsAreProvidedTicketShouldBeInNewStatus()
        {
            var cmd = ImportTicketCommandBuilder.Build();

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(TicketStatusEnum.New, Ticket.Status);
        }

        [Fact]
        public async Task WhenAdvocateDetailsAreProvidedTicketShouldBeAssignedToIt()
        {
            var advocateEmail = "adv@test.com";
            var advocate = new User(Guid.NewGuid(), "FirstName", "LastName");
            Mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmail(advocateEmail))
                .ReturnsAsync(advocate);

            var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1));

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(TicketStatusEnum.Assigned, Ticket.Status);
            Assert.Equal(advocate.Id, Ticket.AdvocateId);
        }

        [Fact]
        public async Task WhenSolvedDateIsProvidedTicketShouldBeInSolvedStatus()
        {
            var advocateEmail = "adv@test.com";
            var advocate = new User(Guid.NewGuid(), "FirstName", "LastName");
            Mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmail(advocateEmail))
                .ReturnsAsync(advocate);

            var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1),
                solvedDate: new DateTime(2020, 1, 2));

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(TicketStatusEnum.Solved, Ticket.Status);
        }

        [Fact]
        public async Task WhenClosedDateIsProvidedTicketShouldBeInClosedStatus()
        {
            var advocateEmail = "adv@test.com";
            var advocate = new User(Guid.NewGuid(), "FirstName", "LastName");
            Mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmail(advocateEmail))
                .ReturnsAsync(advocate);

            var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1),
                solvedDate: new DateTime(2020, 1, 2), closedDate: new DateTime(2020, 1, 3));

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(TicketStatusEnum.Closed, Ticket.Status);
            Assert.Equal(new DateTime(2020, 1, 3), Ticket.ClosedDate);
            Assert.Equal(ClosedBy.System, Ticket.ClosedBy);
        }

        [Fact]
        public async Task WhenCsatIsProvidedTicketItShouldBeSetOnTicket()
        {
            var advocateEmail = "adv@test.com";
            var advocate = new User(Guid.NewGuid(), "FirstName", "LastName");
            Mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmail(advocateEmail))
                .ReturnsAsync(advocate);

            var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1),
                solvedDate: new DateTime(2020, 1, 2), closedDate: new DateTime(2020, 1, 3), csat: 4);

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(4, Ticket.Csat);
            Assert.Equal(new DateTime(2020, 1, 3), Ticket.CsatDate);
        }

        [Fact]
        public async Task WhenComplexityIsProvidedTicketItShouldBeSetOnTicket()
        {
            var advocateEmail = "adv@test.com";
            var advocate = new User(Guid.NewGuid(), "FirstName", "LastName");
            Mocker.GetMock<IUserRepository>()
                .Setup(x => x.GetByEmail(advocateEmail))
                .ReturnsAsync(advocate);

            var cmd = ImportTicketCommandBuilder.Build(advocateEmail: advocateEmail, assignedDate: new DateTime(2020, 1, 1),
                solvedDate: new DateTime(2020, 1, 2), closedDate: new DateTime(2020, 1, 3), complexity: 7);

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(7, Ticket.Complexity);
        }

        [Fact]
        public async Task WhenTagsAreProvidedTheyShouldBeAssignedToTicket()
        {
            var brandId = Guid.NewGuid();
            var tag1 = new Tag(brandId, "tag-1") { Id = Guid.NewGuid() };
            var tag2 = new Tag(brandId, "tag-2") { Id = Guid.NewGuid() };
            var tag3 = new Tag(brandId, "tag-3") { Id = Guid.NewGuid() };
            Mocker.GetMock<IBrandRepository>()
                .Setup(x => x.GetTags(brandId, true, null))
                .ReturnsAsync(new[] { tag1, tag2, tag3 });

            var cmd = ImportTicketCommandBuilder.Build(brandId: brandId, tags: new[] { tag1.Id, tag3.Id });

            var id = await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Contains(Ticket.Tags, x => x.TagId == tag1.Id && x.Level == Ticket.Level);
            Assert.Contains(Ticket.Tags, x => x.TagId == tag3.Id && x.Level == Ticket.Level);
        }
    }
}