using System;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.Extensions.Options;
using Moq;
using Tigerspike.Solv.Application.CommandHandlers;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Events;
using Tigerspike.Solv.Domain.Events.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers.TicketCommandHandlerTests
{
	public class DeleteTicketCommandHandlerTests : BaseCommandHandlerTests<TicketCommandHandler>
	{

		public DeleteTicketCommandHandlerTests()
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
		public async Task ShouldSucceedWhenDeletingPracticeTicket()
		{
			// Arrange
			var id = Guid.NewGuid();
			var cmd = new DeleteTicketCommand(id);

			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, id)
				.With(x => x.IsPractice, true)
				.Build();

			Mocker.GetMock<ITicketRepository>().Setup(x => x.FindAsync(id)).ReturnsAsync(mockTicket);
			Mocker.GetMock<ITicketRepository>().Setup(x => x.Delete(mockTicket));

			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			MediatorMock.Verify(m => m.RaiseEvent(It.Is<TicketDeletedEvent>(dn => dn.TicketId == id)), Times.Once);

			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Key == CommandHandler.CommitErrorKey)),
				Times.Never);
		}

		[Fact]
		public async Task ShoulFailWhenDeletingNonPracticeTicket()
		{
			// Arrange
			var id = Guid.NewGuid();
			var cmd = new DeleteTicketCommand(id);

			var mockTicket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Id, id)
				.With(x => x.IsPractice, false)
				.Build();

			Mocker.GetMock<ITicketRepository>().Setup(x => x.FindAsync(id)).ReturnsAsync(mockTicket);
			Mocker.GetMock<ITicketRepository>().Setup(x => x.Delete(mockTicket));

			Mocker.GetMock<IUnitOfWork>().Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			MediatorMock.Verify(m => m.RaiseEvent(It.Is<TicketDeletedEvent>(dn => dn.TicketId == id)), Times.Never);

			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Value == "Ticket cannot be found or is not in practice")),
				Times.Once);
		}

		[Fact]
		public async Task ShouldFailWhenThereAreNoTickets()
		{
			// Arrange
			var id = Guid.NewGuid();
			var cmd = new DeleteTicketCommand(id);

			Mocker.GetMock<ITicketRepository>().Setup(x => x.FindAsync(id)).ReturnsAsync(null as Ticket);

			// Act
			await SystemUnderTest.Handle(cmd, CancellationToken.None);

			// Assert
			MediatorMock.Verify(m => m.RaiseEvent(It.Is<TicketDeletedEvent>(dn => dn.TicketId == id)), Times.Never);

			MediatorMock.Verify(
				m => m.RaiseEvent(It.Is<DomainNotification>(dn => dn.Value == "Ticket cannot be found or is not in practice")),
				Times.Once);
		}
	}
}