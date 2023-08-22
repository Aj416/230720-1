using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Infra.Bus.Configuration;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Context;
using Tigerspike.Solv.Services.Invoicing.Domain;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Application.Consumers
{
	public class NewCreateAdvocateInvoiceConsumer : IConsumer<ICreateAdvocateInvoiceCommand>
	{
		private readonly IRequestClient<IFetchTicketsForAdvocateInvoiceCommand> _ticketClient;
		private readonly IRequestClient<IFetchBrandIdsForInvoicingCommand> _advocateClient;
		private readonly InvoicingDbContext _dbContext;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly ISequenceRepository _sequenceRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILogger<NewCreateAdvocateInvoiceConsumer> _logger;

		public NewCreateAdvocateInvoiceConsumer(
			IRequestClient<IFetchTicketsForAdvocateInvoiceCommand> ticketClient,
			IRequestClient<IFetchBrandIdsForInvoicingCommand> advocateClient,
			InvoicingDbContext dbContext,
			IAdvocateInvoiceRepository advocateInvoiceRepository,
			IBillingDetailsRepository billingDetailsRepository,
			IInvoicingCycleRepository invoicingCycleRepository,
			ISequenceRepository sequenceRepository,
			IBus bus,
			IOptions<BusOptions> busOptions,
			ILogger<NewCreateAdvocateInvoiceConsumer> logger)
		{
			_ticketClient = ticketClient;
			_advocateClient = advocateClient;
			_dbContext = dbContext;
			_advocateInvoiceRepository = advocateInvoiceRepository;
			_billingDetailsRepository = billingDetailsRepository;
			_invoicingCycleRepository = invoicingCycleRepository;
			_sequenceRepository = sequenceRepository;
			_bus = bus;
			_busOptions = busOptions.Value;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ICreateAdvocateInvoiceCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug("Creating advocate invoice for {advocateId} for cycle {invoicingCycleId}", msg.AdvocateId, msg.InvoicingCycleId);

			if (msg.AdvocateId != Guid.Empty && msg.InvoicingCycleId != Guid.Empty)
			{
				// fetch basic invoice data
				var invoicingCycle = await _invoicingCycleRepository.FindAsync(msg.InvoicingCycleId);
				var (from, to) = (invoicingCycle.From, invoicingCycle.To);
				from = from.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week

				var fetchTicketsForAdvocateInvoiceResult =
					await _ticketClient.GetResponse<IFetchTicketsForAdvocateInvoiceResult>(
						new FetchTicketsForAdvocateInvoiceCommand(from, to, msg.AdvocateId));

				var fetchBrandIdsForInvoicingResult =
					await _advocateClient.GetResponse<IFetchBrandIdsForInvoicingResult>(
						new FetchBrandIdsForInvoicingCommand(msg.AdvocateId));

				var platformBillingDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();

				// create invoice lines for the brands that either:
				// a) advocate has close the tickets for in the period
				// b) advocate is authorized to solve tickets right now
				var ticketLines = fetchTicketsForAdvocateInvoiceResult.Message.TicketsByBrand.Select(b => new AdvocateInvoiceLineItem(b.brandId, b.brandName, b.priceTotal, b.ticketsCount));
				var brandLines = fetchBrandIdsForInvoicingResult.Message.BrandDetails.Select(b => new AdvocateInvoiceLineItem(b.id, b.name, 0m, 0));
				var lineItems = ticketLines.Union(brandLines)
					.GroupBy(x => new { x.BrandId, x.BrandName })
					.Select(x => new AdvocateInvoiceLineItem(x.Key.BrandId, x.Key.BrandName, x.Sum(y => y.Amount), x.Sum(y => y.TicketsCount)))
					.ToList();
				using (var t = await _dbContext.Database.BeginTransactionAsync())
				{
					// fetch sequence number for new invoice
					var advocateSequenceName = $"{nameof(AdvocateInvoice)}-{msg.AdvocateId}";

					var sequence = await _sequenceRepository.Next(advocateSequenceName);

					// create invoice
					var invoice = new AdvocateInvoice(invoicingCycle.Id, platformBillingDetailsId, msg.AdvocateId, sequence, lineItems.ToList());
					await _advocateInvoiceRepository.InsertAsync(invoice);

					// save new invoice
					await _advocateInvoiceRepository.SaveChangesAsync();

					// relate tickets involved in calculations with created invoice
					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Ticket}"));
					await endpoint.Send<ISetTicketsAdvocateInvoiceIdCommand>(new
					{
						AdvocateInvoiceId = invoice.Id,
						invoice.AdvocateId,
						From = from,
						To = to
					});

					await t.CommitAsync();
				}

				_logger.LogDebug($"Invoice for advocate {msg.AdvocateId} created");
			}
			else
			{
				_logger.LogError($"Validation failed for {msg}");
			}
		}
	}
}
