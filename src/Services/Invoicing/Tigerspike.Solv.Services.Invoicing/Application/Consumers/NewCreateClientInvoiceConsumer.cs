using System;
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
	public class NewCreateClientInvoiceConsumer : IConsumer<ICreateClientInvoiceCommand>
	{
		private readonly IRequestClient<IFetchClientInvoicingAmountCommand> _ticketClient;
		private readonly IRequestClient<IFetchBrandBillingDetailCommand> _brandClient;
		private readonly InvoicingDbContext _dbContext;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly ISequenceRepository _sequenceRepository;
		private readonly IBus _bus;
		private readonly BusOptions _busOptions;
		private readonly ILogger<NewCreateClientInvoiceConsumer> _logger;

		public NewCreateClientInvoiceConsumer(
			IRequestClient<IFetchClientInvoicingAmountCommand> ticketClient,
			IRequestClient<IFetchBrandBillingDetailCommand> brandClient,
			InvoicingDbContext dbContext,
			IBillingDetailsRepository billingDetailsRepository,
			IClientInvoiceRepository clientInvoiceRepository,
			IInvoicingCycleRepository invoicingCycleRepository,
			ISequenceRepository sequenceRepository,
			IBus bus,
			IOptions<BusOptions> busOptions,
			ILogger<NewCreateClientInvoiceConsumer> logger)
		{
			_ticketClient = ticketClient;
			_brandClient = brandClient;
			_dbContext = dbContext;
			_billingDetailsRepository = billingDetailsRepository;
			_clientInvoiceRepository = clientInvoiceRepository;
			_invoicingCycleRepository = invoicingCycleRepository;
			_sequenceRepository = sequenceRepository;
			_bus = bus;
			_busOptions = busOptions.Value;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<ICreateClientInvoiceCommand> context)
		{
			var msg = context.Message;
			if (msg.BrandId != Guid.Empty && msg.InvoicingCycleId != Guid.Empty)
			{
				_logger.LogDebug("Creating client invoice for {brandId} for cycle {invoicingCycleId}", msg.BrandId, msg.InvoicingCycleId);

				// fetch basic invoice data
				var invoicingCycle = await _invoicingCycleRepository.FindAsync(msg.InvoicingCycleId);

				var (from, to) = (invoicingCycle.From, invoicingCycle.To);
				from = from.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week

				var clientInvoicingAmountResult =
					await _ticketClient.GetResponse<IFetchClientInvoicingAmountResult>(
						new FetchClientInvoicingAmountCommand(from, to, msg.BrandId));

				var brandBillingDetailResult =
				await _brandClient.GetResponse<IFetchBrandBillingDetailResult>(
					new FetchBrandBillingDetailCommand(msg.BrandId));

				var brandBillingDetailsId = brandBillingDetailResult.Message.BillingDetailsId;
				var platformBillingDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();
				var priceTotal = clientInvoicingAmountResult.Message.PriceTotal;
				var feeTotal = clientInvoicingAmountResult.Message.FeeTotal;
				var ticketsCount = clientInvoicingAmountResult.Message.TicketCount;

				// make calculations
				var subtotal = priceTotal + feeTotal;
				var vatRate = brandBillingDetailResult.Message.VatRate;
				var vatAmount = vatRate.HasValue ? (decimal?)Math.Round(feeTotal * vatRate.Value, 2) : null; // tax calculated only from fees
				var invoiceTotal = feeTotal + (vatAmount ?? 0.00m);
				var paymentTotal = priceTotal + invoiceTotal;

				using (var t = await _dbContext.Database.BeginTransactionAsync())
				{
					// fetch sequence number for new invoice
					var sequence = await _sequenceRepository.Next(nameof(ClientInvoice));

					// create invoice
					var invoice = new ClientInvoice(msg.BrandId, invoicingCycle.Id, priceTotal, feeTotal, subtotal, vatRate, vatAmount, invoiceTotal, paymentTotal, ticketsCount, sequence, platformBillingDetailsId, brandBillingDetailsId);
					await _clientInvoiceRepository.InsertAsync(invoice);

					// save new invoice
					await _clientInvoiceRepository.SaveChangesAsync();

					var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_busOptions.Queues.Ticket}"));
					await endpoint.Send<ISetTicketsClientInvoiceIdCommand>(new
					{
						ClientInvoiceId = invoice.Id,
						invoice.BrandId,
						From = from,
						To = to
					});

					await t.CommitAsync();
				}

				_logger.LogDebug($"Invoice for client {msg.BrandId} created");
			}
			else
			{
				_logger.LogError($"Validation failed for {msg}");
			}
		}
	}
}
