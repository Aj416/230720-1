using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Moq;
using Tigerspike.Solv.Services.Fraud.Application.Commands.TicketDetection;
using Tigerspike.Solv.Services.Fraud.Application.Events;
using Tigerspike.Solv.Services.Fraud.Domain;
using Tigerspike.Solv.Services.Fraud.Enum;
using Xunit;

namespace Tigerspike.Solv.Services.Fraud.Tests.CommandsHandlers.TicketDetectionCommandHandlerTests
{
	public class CreateTicketDetectionCommandTests : BaseClass
	{
		[Fact]
		public async Task ShouldInsertBothTicketAndDetectionIntoDatabaseWhenRules()
		{
			// Arrange
			var expectedTicketDetection = GetMockTicketDetectionWithAppliedRules();
			var expectedTicket = GetMockTicket();

			var cmd = new CreateTicketDetectionCommand(expectedTicketDetection.Rules.Select(etd => Guid.Parse(etd)).ToList(),
				Guid.Parse(expectedTicketDetection.TicketId),
				(int)expectedTicketDetection.Status);

			MockTicketRepository.Setup(r => r.GetTicket(It.IsAny<string>()))
				.Returns(expectedTicket);

			MockTicketDetectionRepository.Setup(r => r.GetTicketDetection(It.IsAny<string>()))
				.Returns(expectedTicketDetection);

			MockRuleRepository.Setup(r => r.GetByRiskLevel(It.Is<RiskLevel>(rl => rl == RiskLevel.Low)))
				.Returns(new List<string>());

			// Act
			await TicketDetectionCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockTicketRepository.Verify(m =>
					m.AddOrUpdateTicket(It.Is<Ticket>(
						actualApp => actualApp.AssignedTo == expectedTicket.AssignedTo
									 && actualApp.TicketId == expectedTicket.TicketId
									 && actualApp.CustomerId == expectedTicket.CustomerId
									 && actualApp.BrandId == expectedTicket.BrandId
									 && actualApp.BrandName == expectedTicket.BrandName))
				, Times.Once);


			MockTicketDetectionRepository.Verify(m =>
					m.AddOrUpdateDetection(It.Is<TicketDetection>(
						actual => actual.TicketId == expectedTicketDetection.TicketId
									 && actual.Status == expectedTicketDetection.Status
									 && actual.Rules == expectedTicketDetection.Rules))
				, Times.Once);

			MockMediator.Verify(
				m => m.RaiseEvent(It.IsAny<TicketDetectionCreatedEvent>()),
				Times.Once);
		}

		[Fact]
		public async Task ShouldOnlyInsertDetectionIntoDatabaseWhenNoRules()
		{
			// Arrange
			var expectedTicketDetection = GetMockTicketDetectionWithoutAppliedRules();
			var expectedTicket = GetMockTicket();

			var cmd = new CreateTicketDetectionCommand(expectedTicketDetection.Rules.Select(etd => Guid.Parse(etd)).ToList(),
				Guid.Parse(expectedTicketDetection.TicketId),
				(int)expectedTicketDetection.Status);

			MockTicketRepository.Setup(r => r.GetTicket(It.IsAny<string>()))
				.Returns(expectedTicket);

			MockTicketDetectionRepository.Setup(r => r.GetTicketDetection(It.IsAny<string>()))
				.Returns(expectedTicketDetection);

			MockRuleRepository.Setup(r => r.GetByRiskLevel(It.Is<RiskLevel>(rl => rl == RiskLevel.Low)))
				.Returns(new List<string>());

			// Act
			await TicketDetectionCommandHandler.Handle(cmd, CancellationToken.None);

			// Assert
			MockTicketRepository.Verify(m =>
					m.AddOrUpdateTicket(It.Is<Ticket>(
						actualApp => actualApp.AssignedTo == expectedTicket.AssignedTo
									 && actualApp.TicketId == expectedTicket.TicketId
									 && actualApp.CustomerId == expectedTicket.CustomerId
									 && actualApp.BrandId == expectedTicket.BrandId
									 && actualApp.BrandName == expectedTicket.BrandName))
				, Times.Never);


			MockTicketDetectionRepository.Verify(m =>
					m.AddOrUpdateDetection(It.Is<TicketDetection>(
						actual => actual.TicketId == expectedTicketDetection.TicketId
									 && actual.Status == expectedTicketDetection.Status
									 && actual.Rules == expectedTicketDetection.Rules))
				, Times.Once);

			MockMediator.Verify(
				m => m.RaiseEvent(It.IsAny<TicketDetectionCreatedEvent>()),
				Times.Once);
		}
	}
}