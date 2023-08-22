using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class GenerateInvoicingCycleInvoicesConsumer : IConsumer<GenerateInvoicingCycleInvoices>
	{
		private readonly IBrandRepository _brandRepository;
		private readonly ILogger<GenerateInvoicingCycleInvoicesConsumer> _logger;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly ITicketRepository _ticketRepository;

		public GenerateInvoicingCycleInvoicesConsumer(
			ILogger<GenerateInvoicingCycleInvoicesConsumer> logger,
			IAdvocateRepository advocateRepository,
			ITicketRepository ticketRepository,
			IBrandRepository brandRepository)
		{
			_brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
			_logger = logger;
			_advocateRepository = advocateRepository ?? throw new ArgumentNullException(nameof(advocateRepository));
			_ticketRepository = ticketRepository;
		}

		public async Task Consume(ConsumeContext<GenerateInvoicingCycleInvoices> context)
		{
			_logger.LogDebug("Started consuming invoicing cycle {invoicingCycleId}", context.Message.InvoicingCycleId);
			var notification = context.Message;
			var from = notification.From.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week
			var to = notification.To;

			var brands = await _brandRepository.GetBrandIdsForInvoicing();
			var clientCommands = brands.Select(x => new CreateClientInvoiceCommand(x, notification.InvoicingCycleId)).ToList();
			_logger.LogDebug("Scheduling creation of {count} client invoices", clientCommands.Count);

			// Send a command for each brand to generate a client invoice.
			foreach (var cmd in clientCommands)
			{
				await context.Send(cmd);
			}

			var advocatesWithAuthorisedBrands = await _advocateRepository.GetAllAdvocateIdsForInvoicing();
			var advocatesWithTickets = await _ticketRepository.GetAdvocatesToInvoice(from, to);
			var advocatesToInvoice = Enumerable.Union(advocatesWithAuthorisedBrands, advocatesWithTickets);

			var advocateCommands = advocatesToInvoice.Select(x => new CreateAdvocateInvoiceCommand(x, notification.InvoicingCycleId)).ToList();
			_logger.LogDebug("Scheduling creation of {count} advocate invoices", advocateCommands.Count);

			// Send a command for each advocate to generate a advocate invoice.
			foreach (var cmd in advocateCommands)
			{
				await context.Send(cmd);
			}
		}

	}
}