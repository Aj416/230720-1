using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class StartInvoicingCycleConsumer : IConsumer<StartInvoicingCycleCommand>
	{
		private readonly InvoicingOptions _invoicingOptions;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly ILogger<StartInvoicingCycleConsumer> _logger;
		private readonly IWebHostEnvironment _environment;

		public StartInvoicingCycleConsumer(
			IOptions<InvoicingOptions> invoicingOptions,
			IInvoicingCycleRepository invoicingCycleRepository,
			IWebHostEnvironment environment,
			ILogger<StartInvoicingCycleConsumer> logger)
		{
			_invoicingOptions = invoicingOptions?.Value ?? throw new ArgumentNullException(nameof(invoicingOptions));
			_invoicingCycleRepository = invoicingCycleRepository ??
			                            throw new ArgumentNullException(nameof(invoicingCycleRepository));
			_environment = environment;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Consume(ConsumeContext<StartInvoicingCycleCommand> context)
		{
			if (!context.Message.IsValid())
			{
				throw new InvalidOperationException("Validation failed.");
			}

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
				throw new InvalidOperationException($"Validation failed for {context.Message}. The invoicing cycle date is not valid.");
			}

			// Create the new invoicing cycle
			var invoiceCycle = new InvoicingCycle(invoicingCycleStartDate, invoicingCycleEndDate);
			await _invoicingCycleRepository.InsertAsync(invoiceCycle);
			await _invoicingCycleRepository.SaveChangesAsync();

			_logger.LogInformation("Created invoicing cycle: {invoicingCycleId}", invoiceCycle.Id);
			await context.Send(new GenerateInvoicingCycleInvoices(invoiceCycle.Id, invoiceCycle.From, invoiceCycle.To));
			await context.RespondAsync(new StartInvoicingCycleResult
			{
				Success = true,
				Id = invoiceCycle.Id
			});
		}
	}
}