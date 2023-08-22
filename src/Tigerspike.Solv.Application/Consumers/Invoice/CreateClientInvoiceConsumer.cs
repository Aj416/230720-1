using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class CreateClientInvoiceConsumer : IConsumer<CreateClientInvoiceCommand>
	{
		private readonly SolvDbContext _dbContext;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly ISequenceRepository _sequenceRepository;
		private readonly IBrandRepository _brandRepository;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly ILogger<StartInvoicingCycleConsumer> _logger;

		public CreateClientInvoiceConsumer(
			SolvDbContext dbContext,
			IClientInvoiceRepository clientInvoiceRepository,
			IInvoicingCycleRepository invoicingCycleRepository,
			ITicketRepository ticketRepository,
			ISequenceRepository sequenceRepository,
			IBrandRepository brandRepository,
			IBillingDetailsRepository billingDetailsRepository,
			ILogger<StartInvoicingCycleConsumer> logger)
		{
			_invoicingCycleRepository = invoicingCycleRepository;
			_ticketRepository = ticketRepository;
			_sequenceRepository = sequenceRepository;
			_brandRepository = brandRepository;
			_billingDetailsRepository = billingDetailsRepository;
			_dbContext = dbContext;
			_clientInvoiceRepository = clientInvoiceRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<CreateClientInvoiceCommand> context)
		{
			if (context.Message.IsValid())
			{
				_logger.LogDebug("Creating client invoice for {brandId} for cycle {invoicingCycleId}", context.Message.BrandId, context.Message.InvoicingCycleId);

				// fetch basic invoice data
				var invoicingCycle = await _invoicingCycleRepository.FindAsync(context.Message.InvoicingCycleId);
				var (from, to) = (invoicingCycle.From, invoicingCycle.To);
				from = from.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week
				var (priceTotal, feeTotal, ticketsCount) = await _ticketRepository.GetClientInvoicingAmounts(from, to, context.Message.BrandId);
				var brandDetails = await _brandRepository.GetFirstOrDefaultAsync(x => new { x.BillingDetailsId, x.VatRate }, x => x.Id == context.Message.BrandId);
				var brandBillingDetailsId = brandDetails.BillingDetailsId;
				var platformBillingDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();

				// make calculations
				var subtotal = priceTotal + feeTotal;
				var vatRate = brandDetails.VatRate;
				var vatAmount = vatRate.HasValue ? (decimal?)Math.Round(feeTotal * vatRate.Value, 2) : null; // tax calculated only from fees
				var invoiceTotal = feeTotal + (vatAmount ?? 0.00m);
				var paymentTotal = priceTotal + invoiceTotal;


				using (var t = await _dbContext.Database.BeginTransactionAsync())
				{
					// fetch sequence number for new invoice
					var sequence = await _sequenceRepository.Next(nameof(ClientInvoice));

					// create invoice
					var invoice = new ClientInvoice(context.Message.BrandId, invoicingCycle.Id, priceTotal, feeTotal, subtotal, vatRate, vatAmount, invoiceTotal, paymentTotal, ticketsCount, sequence, platformBillingDetailsId, brandBillingDetailsId);
					await _clientInvoiceRepository.InsertAsync(invoice);

					// save new invoice
					await _clientInvoiceRepository.SaveChangesAsync();

					// relate tickets involved in calculations with created invoice
					await _ticketRepository.SetTicketsClientInvoiceId(invoice.Id, invoice.BrandId, from, to);
					await t.CommitAsync();
				}

				_logger.LogDebug("Invoice for client {brandId} created", context.Message.BrandId);
			}
			else
			{
				_logger.LogError("Validation failed for {message}", context.Message);
			}
		}
	}
}