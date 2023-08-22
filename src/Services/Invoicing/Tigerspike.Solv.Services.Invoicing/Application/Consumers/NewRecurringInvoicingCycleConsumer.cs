using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Application.Services;

namespace Tigerspike.Solv.Services.Invoicing.Application.Consumers
{
	public class NewRecurringInvoicingCycleConsumer : IConsumer<NewRecurringInvoicingCycleCommand>
	{
		private readonly ITimestampService _timestampService;
		private readonly ILogger<NewRecurringInvoicingCycleConsumer> _logger;

		public NewRecurringInvoicingCycleConsumer(
			ITimestampService timestampService,
			ILogger<NewRecurringInvoicingCycleConsumer> logger)
		{
			_timestampService = timestampService;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<NewRecurringInvoicingCycleCommand> context)
		{
			var startDate = _timestampService.GetUtcTimestamp().Date.AddDays(-7);
			_logger.LogInformation("Recurring schedule: starting new invoicing cycle for the start date: {startDate}", startDate);

			await context.Send<IStartInvoicingCycleCommand>(
				new
				{
					StartDate = startDate
				});
		}
	}
}
