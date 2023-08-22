using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Domain.Commands.Invoice;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class RecurringInvoicingCycleConsumer : IConsumer<RecurringInvoicingCycleCommand>
	{
		private readonly ITimestampService _timestampService;
		private readonly ILogger<RecurringInvoicingCycleConsumer> _logger;

		public RecurringInvoicingCycleConsumer(
			ITimestampService timestampService,
			ILogger<RecurringInvoicingCycleConsumer> logger)
		{
			_timestampService = timestampService;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<RecurringInvoicingCycleCommand> context)
		{
			var startDate = _timestampService.GetUtcTimestamp().Date.AddDays(-7);
			_logger.LogInformation("Recurring schedule: starting new invoicing cycle for the start date: {startDate}", startDate);

			var cmd = new StartInvoicingCycleCommand(startDate);
			await context.Send(cmd);
		}
	}
}