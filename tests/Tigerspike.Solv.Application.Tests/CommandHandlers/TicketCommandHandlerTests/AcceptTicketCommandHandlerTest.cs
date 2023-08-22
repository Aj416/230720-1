﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Commands;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;


namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
    public class AcceptTicketCommandHandlerTest : BaseCommandHandlerTests<TicketCommandHandler>
    {
        public AcceptTicketCommandHandlerTest()
        {
            Mocker.GetMock<IOptions<EmailTemplatesOptions>>()
                .Setup(s => s.Value)
                .Returns(new EmailTemplatesOptions
                {
                    AdvocateExportEmailSubject = "subject",
                    EmailLogoLocation = "location",
                    AdvocateExportEmailAttachmentContentType = "type",
                    AdvocateExportEmailAttachmentFileName = "filename"
                });
        }
        [Fact]
        public async Task TicketStatusShouldChangeWhenIsAccepted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var cmd = new AcceptTicketCommand(id);
            var userId = Guid.NewGuid();
            var advocateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var mockUser = Builder<User>
                .CreateNew()
                .With(x => x.Id, userId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Unit")
                .With(x => x.Enabled, true)
                .Build();
            var mockCustomer = Builder<User>
                .CreateNew()
                .With(x => x.Id, customerId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Customer")
                .Build();
            var mockAdovcateApplication = Builder<AdvocateApplication>
               .CreateNew()
               .With(x => x.Id, advocateId)
               .With(x => x.ApplicationStatus, Domain.Enums.AdvocateApplicationStatus.AccountCreated)
               .Build();

            var mockBrand = Builder<Brand>
                .CreateNew()
                .With(x => x.Id, brandId)
                .With(x => x.Name, "test brand")
                .With(x => x.TicketPrice, 10m)
                .With(x => x.FeePercentage, 0.3m)
                .Build();

            var mockAdvocateBrand = Builder<AdvocateBrand>
                .CreateNew()
                .With(x => x.AdvocateId, advocateId)
                .With(x => x.BrandId, brandId)
                .With(x => x.Brand, mockBrand)
                .With(x => x.AgreementAccepted,true)
                .With(x => x.Authorized, true)
                .With(x => x.ContractAccepted, true)
                .With(x => x.User, mockUser)
                .With(x => x.Enabled, true)
                .With(x => x.CreatedDate, DateTime.UtcNow)
                .Build();
            var brands = new List<AdvocateBrand>();
            brands.Add(mockAdvocateBrand);
            var mockAdovcate = Builder<Advocate>
                .CreateNew()
                .With(x => x.Id, advocateId)
                .With(x => x.User, mockUser)
                .With(x => x.Brands, brands)
                .With(x => x.PracticeComplete, true)
                .With(x => x.VideoWatched, true)
                .Build();
            var mockTicket = Builder<Ticket>
                .CreateNew()
                .With(x => x.Id, id)
                .With(x => x.AdvocateId, advocateId)
                .With(x => x.Status, Domain.Enums.TicketStatusEnum.Reserved)
                .With(x => x.Price, (decimal)7m)
                .With(x => x.Fee, (decimal)3m)
                .With(x => x.BrandId, brandId)
                .With(x => x.IsPractice, false)
                .With(x => x.Question, "Test Question")
                .With(x => x.StatusHistory = new List<TicketStatusHistory>())
                .With(x => x.RejectionHistory = new List<TicketRejectionHistory>())
                .With(x => x.TrackingHistory = new List<TrackingDetail>())
                .With(x => x.Customer, mockCustomer)
                .With(x=> x.FirstAssignedDate , DateTime.UtcNow)
                .With(x => x.ReservationExpiryDate, DateTime.UtcNow.AddSeconds(30))
                .With(x => x.Advocate, mockAdovcate)
                .Build();

            Mocker.GetMock<IBrandRepository>().Setup(x => x.FindAsync(brandId)).ReturnsAsync(mockBrand);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(userId)).ReturnsAsync(mockUser);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(customerId)).ReturnsAsync(mockCustomer);
            Mocker.GetMock<IAdvocateBrandRepository>().Setup(x => x.FindAsync(advocateId)).ReturnsAsync(mockAdvocateBrand);
            Mocker.GetMock<IBrandService>().Setup(x => x.CalculateTicketFee(mockBrand.TicketPrice, mockBrand.FeePercentage)).Returns(3m);
            Mocker.GetMock<ITicketRepository>().Setup(x => x.GetFullTicket(It.IsAny<Expression<Func<Ticket, bool>>>())).Returns(Task.FromResult(mockTicket));
            Mocker.GetMock<IMediatorHandler>().Setup(x => x.SendCommand<Ticket>(It.IsAny<Command<Ticket>>())).Returns(Task.FromResult(mockTicket));
            Mocker.GetMock<IAdvocateRepository>().Setup(x => x.FindAsync(advocateId)).ReturnsAsync(mockAdovcate);
            Mocker.GetMock<IAdvocateRepository>().Setup(x => x.GetFullAdvocateAsync(It.IsAny<Expression<Func<Advocate, bool>>>())).Returns(Task.FromResult(mockAdovcate));
            Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);
            // Act
            await SystemUnderTest.Handle(cmd, CancellationToken.None);
            // Assert
            Assert.Equal(Domain.Enums.TicketStatusEnum.Assigned, mockTicket.Status);
            Mocker.GetMock<IMediatorHandler>().Verify(x => x.RaiseEvent(It.IsAny<TicketAcceptedEvent>()), Times.Once);

        }

        [Fact]
        public async Task AcceptTicketCommandShouldThrowExceptionWhenReservationExipred()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var cmd = new AcceptTicketCommand(id);
            var userId = Guid.NewGuid();
            var advocateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var mockUser = Builder<User>
                .CreateNew()
                .With(x => x.Id, userId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Unit")
                .With(x => x.Enabled, true)
                .Build();
            var mockCustomer = Builder<User>
                .CreateNew()
                .With(x => x.Id, customerId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Customer")
                .Build();
            var mockAdovcateApplication = Builder<AdvocateApplication>
               .CreateNew()
               .With(x => x.Id, advocateId)
               .With(x => x.ApplicationStatus, Domain.Enums.AdvocateApplicationStatus.AccountCreated)
               .Build();

            var mockBrand = Builder<Brand>
                .CreateNew()
                .With(x => x.Id, brandId)
                .With(x => x.Name, "test brand")
                .With(x => x.TicketPrice, 10m)
                .With(x => x.FeePercentage, 0.3m)
                .Build();

            var mockAdvocateBrand = Builder<AdvocateBrand>
                .CreateNew()
                .With(x => x.AdvocateId, advocateId)
                .With(x => x.BrandId, brandId)
                .With(x => x.Brand, mockBrand)
                .With(x => x.AgreementAccepted, true)
                .With(x => x.Authorized, true)
                .With(x => x.ContractAccepted, true)
                .With(x => x.User, mockUser)
                .With(x => x.Enabled, true)
                .With(x => x.CreatedDate, DateTime.UtcNow)
                .Build();
           var brands = new List<AdvocateBrand>();
            brands.Add(mockAdvocateBrand);
            var mockAdovcate = Builder<Advocate>
                .CreateNew()
                .With(x => x.Id, advocateId)
                .With(x => x.User, mockUser)
                .With(x => x.Brands, brands)
                .With(x => x.PracticeComplete, true)
                .With(x => x.VideoWatched, true)
                .Build();
            var mockTicket = Builder<Ticket>
                .CreateNew()
                .With(x => x.Id, id)
                .With(x => x.AdvocateId, advocateId)
                .With(x => x.Status, Domain.Enums.TicketStatusEnum.Reserved)
                .With(x => x.Price, (decimal)7m)
                .With(x => x.Fee, (decimal)3m)
                .With(x => x.BrandId, brandId)
                .With(x => x.IsPractice, false)
                .With(x => x.Question, "Test Question")
                .With(x => x.StatusHistory = new List<TicketStatusHistory>())
                .With(x => x.RejectionHistory = new List<TicketRejectionHistory>())
                .With(x => x.Customer, mockCustomer)
                .With(x => x.FirstAssignedDate, DateTime.UtcNow.AddMinutes(-1))
                .With(x => x.ReservationExpiryDate, DateTime.UtcNow.AddSeconds(-30))
                .With(x => x.Advocate, mockAdovcate)
                .Build();

            Mocker.GetMock<IBrandRepository>().Setup(x => x.FindAsync(brandId)).ReturnsAsync(mockBrand);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(userId)).ReturnsAsync(mockUser);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(customerId)).ReturnsAsync(mockCustomer);
            Mocker.GetMock<IAdvocateBrandRepository>().Setup(x => x.FindAsync(advocateId)).ReturnsAsync(mockAdvocateBrand);
            Mocker.GetMock<IBrandService>().Setup(x => x.CalculateTicketFee(mockBrand.TicketPrice, mockBrand.FeePercentage)).Returns(3m);
            Mocker.GetMock<ITicketRepository>().Setup(x => x.GetFullTicket(It.IsAny<Expression<Func<Ticket, bool>>>())).Returns(Task.FromResult(mockTicket));
            Mocker.GetMock<IAdvocateRepository>().Setup(x => x.FindAsync(advocateId)).ReturnsAsync(mockAdovcate);
            Mocker.GetMock<IAdvocateRepository>().Setup(x => x.GetFullAdvocateAsync(It.IsAny<Expression<Func<Advocate, bool>>>())).Returns(Task.FromResult(mockAdovcate));
            Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));
            // Act

            await Assert.ThrowsAsync<DomainException>(() => SystemUnderTest.Handle(cmd, CancellationToken.None));
        }
    }
}
