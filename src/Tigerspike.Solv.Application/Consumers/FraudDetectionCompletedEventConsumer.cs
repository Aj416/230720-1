using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Fraud;

namespace Tigerspike.Solv.Application.Consumers
{
	public class FraudDetectionCompletedEventConsumer : IConsumer<IFraudDetectionCompletedEvent>
	{
		private readonly ILogger<FraudDetectionCompletedEventConsumer> _logger;
		private readonly ITicketRepository _ticketRepository;
		private readonly IMediatorHandler _mediator;

		public FraudDetectionCompletedEventConsumer(
			ITicketRepository ticketRepository,
			IMediatorHandler mediator,
			ILogger<FraudDetectionCompletedEventConsumer> logger
		)
		{
			_ticketRepository = ticketRepository;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IFraudDetectionCompletedEvent> context)
		{
			_logger.LogInformation(
				$"Ticket fraud detection completed for {context.Message.TicketId} as {context.Message.IsFraudSuspected} ");

			await _mediator.SendCommand(new UpdateTicketFraudCommand(context.Message.TicketId,
				context.Message.IsFraudSuspected,
				context.Message.RiskLevel, context.Message.Risks));
		}
	}
}