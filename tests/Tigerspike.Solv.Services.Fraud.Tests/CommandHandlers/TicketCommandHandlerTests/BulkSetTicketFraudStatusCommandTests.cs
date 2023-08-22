using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Services.Fraud.Application.Commands.Ticket;
using Tigerspike.Solv.Services.Fraud.Application.Events;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.CommandsHandlers.TicketCommandHandlerTests
{
	public class BulkSetTicketFraudStatusCommandTests : BaseClass
	{
		[Fact]
		public async Task ShoulFailWhenUpdating()
		{
			var newStatus = FraudStatus.FraudSuspected;
			var items = new List<Guid>();

			var cmd = new BulkSetTicketFraudStatusCommand(newStatus, items);

			MockTicketRepository.Setup(ur => ur.GetTicket(It.IsAny<string>()))
				.Returns((Ticket)null);

			// Act
			await TicketCommandHandler.Handle(cmd, CancellationToken.None);

			MockMediator.Verify(
				m => m.RaiseEvent(It.IsAny<TicketFraudStatusSetEvent>()),
				Times.Never);
		}

		[Fact]
		public async Task ShoulSuceedWhenUpdating()
		{
			var newStatus = FraudStatus.FraudSuspected;
			var items = new List<Guid>(){
				Builder<Guid>.CreateNew()
				.Build(),
				Builder<Guid>.CreateNew()
				.Build()
			};

			var ticket = GetMockTicket();

			var cmd = new BulkSetTicketFraudStatusCommand(newStatus, items);

			MockTicketRepository.Setup(ur => ur.GetTicket(It.IsAny<string>()))
				.Returns(ticket);

			// Act
			await TicketCommandHandler.Handle(cmd, CancellationToken.None);

			MockMediator.Verify(
				m => m.RaiseEvent(It.IsAny<TicketFraudStatusSetEvent>()),
				Times.Once);
		}
	}
}