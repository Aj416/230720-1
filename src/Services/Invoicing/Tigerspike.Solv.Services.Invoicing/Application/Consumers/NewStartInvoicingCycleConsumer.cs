using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;

namespace Tigerspike.Solv.Services.Invoicing.Application.Consumers
{
	public class NewStartInvoicingCycleConsumer :
		IConsumer<IStartInvoicingCycleCommand>
	{
		private readonly InvoicingOptions _invoicingOptions;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<NewStartInvoicingCycleConsumer> _logger;

		public NewStartInvoicingCycleConsumer(
			IOptions<InvoicingOptions> invoicingOptions,
			IInvoicingCycleRepository invoicingCycleRepository,
			IMediatorHandler mediator,
			ILogger<NewStartInvoicingCycleConsumer> logger)
		{
			_invoicingOptions = invoicingOptions?.Value;
			_invoicingCycleRepository = invoicingCycleRepository;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IStartInvoicingCycleCommand> context)
		{
			_logger.LogInformation("StartInvoicingCycleCommand received successfully at {0}", DateTime.UtcNow);

			var periodicity = _invoicingOptions.Periodicity;
			if (periodicity != Periodicity.Weekly)
			{
				throw new InvalidOperationException("Only weekly invoice generating is supported currently");
			}

			// Generate weekly period
			var invoicingCycleStartDate = context.Message.StartDate.Date.StartOfWeek(DayOfWeek.Monday);
			var invoicingCycleEndDate = invoicingCycleStartDate.AddDays(7);

			// Check the current cycle is closed and no invoicing cycle has been created already
			DateTime? lastInvoicingCycleEndDate = await _invoicingCycleRepository.GetFirstOrDefaultAsync(s => s.To, orderBy: i => i.OrderByDescending(s => s.To));

			if (lastInvoicingCycleEndDate != null && invoicingCycleStartDate < lastInvoicingCycleEndDate)
			{
				throw new InvalidOperationException($"Validation failed for {GetType().Name}. The invoicing cycle date is not valid.");
			}

			var invoicingId = await _mediator.SendCommand(new CreateInvoicingCycleCommand(invoicingCycleStartDate, invoicingCycleEndDate));

			await context.Send<IGenerateInvoicingCycleInvoicesCommand>(
				new
				{
					InvoicingCycleId = invoicingId,
					From = invoicingCycleStartDate,
					To = invoicingCycleEndDate
				});

			await context.RespondAsync<IStartInvoicingCycleResult>(new
			{
				Success = invoicingId != null,
				Id = invoicingId ?? Guid.Empty
			});
		}
	}
}
