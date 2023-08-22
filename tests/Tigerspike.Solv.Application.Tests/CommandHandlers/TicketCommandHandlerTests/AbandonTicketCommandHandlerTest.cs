using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Tests.CommandsHandlers;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
    public class AbandonTicketCommandHandlerTest : BaseCommandHandlerTests<TicketCommandHandler>
    {

        public AbandonTicketCommandHandlerTest()
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
        public async Task ShouldRecalculateTicketPricingWhenTicketIsAbandoned()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var cmd = new AbandonTicketCommand(id, new Guid[] {  });
            var userId = Guid.NewGuid();
            var advocateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var mockUser = Builder<User>
                .CreateNew()
                .With(x => x.Id, userId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Unit")
                .Build();
            var mockCustomer = Builder<User>
                .CreateNew()
                .With(x => x.Id, customerId)
                .With(x => x.FirstName, "test")
                .With(x => x.LastName, "Customer")
                .Build();

            var mockAdovcate = Builder<Advocate>
                .CreateNew()
                .With(x => x.Id, advocateId)
                .With(x => x.User, mockUser)
                .Build();
            var mockBrand = Builder<Brand>
                .CreateNew()
                .With(x => x.Id, brandId)
                .With(x => x.Name, "test brand")
                .With(x => x.TicketPrice, 7m)
                .With(x => x.FeePercentage, 0.3m)
				.With(x => x.AbandonReasons, new List<AbandonReason>())
                .Build();
            var mockTicket = Builder<Ticket>
                .CreateNew()
                .With(x => x.Id, id)
                .With(x => x.AdvocateId, advocateId)
                .With(x => x.Status, Domain.Enums.TicketStatusEnum.Assigned)
                .With(x => x.Price, (decimal)3.5m)
                .With(x => x.Fee, (decimal)1.5m)
                .With(x => x.BrandId, brandId)
                .With(x => x.IsPractice, false)
                .With(x => x.Question, "Test Question")
                .With(x => x.Tags, new List<TicketTag>())
                .With(x => x.StatusHistory, new [] {
                        new TicketStatusHistory(Guid.NewGuid(), Domain.Enums.TicketStatusEnum.New, new DateTime(2020, 1, 1), advocateId, TicketLevel.Regular),
                        new TicketStatusHistory(Guid.NewGuid(), Domain.Enums.TicketStatusEnum.Reserved, new DateTime(2020, 1, 2), advocateId, TicketLevel.Regular),
                        new TicketStatusHistory(Guid.NewGuid(), Domain.Enums.TicketStatusEnum.Assigned, new DateTime(2020, 1, 3), advocateId, TicketLevel.Regular) 
                    }.ToList()
                )
                .With(x => x.RejectionHistory = new List<TicketRejectionHistory>())
                .With(x => x.AbandonHistory = new List<TicketAbandonHistory>())
                .With(x => x.Customer, mockCustomer)
                .With(x => x.Advocate , mockAdovcate)
                .Build();

            Mocker.GetMock<IBrandRepository>().Setup(x => x.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Brand, bool>>>(),
                    It.IsAny<Func<IQueryable<Brand>, IOrderedQueryable<Brand>>>(),
                    It.IsAny<Func<IQueryable<Brand>, IIncludableQueryable<Brand, object>>>(),
                    It.IsAny<bool>(), It.IsAny<bool>())
            ).ReturnsAsync(mockBrand);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(userId)).ReturnsAsync(mockUser);
            Mocker.GetMock<IUserRepository>().Setup(x => x.FindAsync(customerId)).ReturnsAsync(mockCustomer);
            Mocker.GetMock<IAdvocateRepository>().Setup(x => x.FindAsync(advocateId)).ReturnsAsync(mockAdovcate);
            Mocker.GetMock<IBrandService>().Setup(x => x.CalculateTicketFee(mockBrand.TicketPrice, mockBrand.FeePercentage)).Returns(3m);
            Mocker.GetMock<ITicketRepository>().Setup(x => x.GetFullTicket(It.IsAny<Expression<Func<Ticket, bool>>>())).Returns(Task.FromResult(mockTicket));
            Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            // Act
            await SystemUnderTest.Handle(cmd, CancellationToken.None);
            Assert.Equal(3m, mockTicket.Fee);
            Assert.Equal(mockBrand.TicketPrice, mockTicket.Price);
        }


    }
}
