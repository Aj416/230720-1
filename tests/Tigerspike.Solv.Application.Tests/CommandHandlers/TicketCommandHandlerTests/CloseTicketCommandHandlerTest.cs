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
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Exceptions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;


namespace Tigerspike.Solv.Application.Tests.CommandHandlers.TicketCommandHandlerTests
{
	public class CloseTicketCommandHandlerTest : BaseCommandHandlerTests<TicketCommandHandler>
	{
		public CloseTicketCommandHandlerTest()
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
		public async Task TicketStatusShouldChangeWhenTicketIsClosed()
		{
			// Arrange
			var id = Guid.NewGuid();
			var brandId = Guid.NewGuid();

			var cmd = new CloseTicketCommand(id, ClosedBy.Customer);
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
			var mockTicketHistory1 = Builder<TicketStatusHistory>
				.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.TicketId, id)
				.With(x => x.Status, TicketStatusEnum.Reserved)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-1))
				.Build();

			var mockTicketHistory2 = Builder<TicketStatusHistory>
				.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.TicketId, id)
				.With(x => x.Status, TicketStatusEnum.Assigned)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-1))
				.Build();
			var mockTicketHistory3 = Builder<TicketStatusHistory>
				.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.With(x => x.TicketId, id)
				.With(x => x.Status, TicketStatusEnum.Solved)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.CreatedDate, DateTime.UtcNow.AddDays(-1))
				.Build();
			var ticketStatusHistories = new List<TicketStatusHistory>();
			ticketStatusHistories.Add(mockTicketHistory1);
			ticketStatusHistories.Add(mockTicketHistory2);
			ticketStatusHistories.Add(mockTicketHistory3);
			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, id)
				.With(x => x.AdvocateId, advocateId)
				.With(x => x.Status, TicketStatusEnum.Solved)
				.With(x => x.Price, (decimal)7m)
				.With(x => x.Fee, (decimal)3m)
				.With(x => x.BrandId, brandId)
				.With(x => x.IsPractice, false)
				.With(x => x.RejectionCount, 0)
				.With(x => x.Question, "Test Question")
				.With(x => x.StatusHistory = ticketStatusHistories)
				.With(x => x.RejectionHistory = new List<TicketRejectionHistory>())
				.With(x => x.TrackingHistory = new List<TrackingDetail>())
				.With(x => x.Customer, mockCustomer)
				.With(x => x.FirstAssignedDate, DateTime.UtcNow.AddDays(-1))
				.With(x => x.ReservationExpiryDate, DateTime.UtcNow.AddSeconds(30).AddDays(-1))
				.With(x => x.Advocate, mockAdovcate)
				.Build();

			Mocker.GetMock<ITicketRepository>().Setup(x => x.GetFullTicket(It.IsAny<Expression<Func<Ticket, bool>>>())).Returns(Task.FromResult(mockTicket));
			Mocker.GetMock<IAdvocateRepository>().Setup(x => x.GetFirstOrDefaultAsync(
				It.IsAny<Expression<Func<Advocate, Advocate>>>(),
				It.IsAny<Expression<Func<Advocate, bool>>>(),
				It.IsAny<Func<IQueryable<Advocate>, IOrderedQueryable<Advocate>>>(),
				It.IsAny<Func<IQueryable<Advocate>, IIncludableQueryable<Advocate, object>>>(),
				It.IsAny<bool>(), It.IsAny<bool>())
				).ReturnsAsync(mockAdovcate);
			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));
			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);
			Assert.Equal(TicketStatusEnum.Closed, mockTicket.Status);
			Mocker.GetMock<IMediatorHandler>().Verify(x => x.RaiseEvent(It.IsAny<TicketClosedEvent>()), Times.Once);
		}

	}
}
