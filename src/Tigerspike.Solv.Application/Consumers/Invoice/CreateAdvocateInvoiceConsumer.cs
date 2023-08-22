using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Context;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Consumers.Invoice
{
	public class CreateAdvocateInvoiceConsumer : IConsumer<CreateAdvocateInvoiceCommand>
	{
		private readonly SolvDbContext _dbContext;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IBillingDetailsRepository _billingDetailsRepository;
		private readonly ITicketRepository _ticketRepository;
		private readonly ISequenceRepository _sequenceRepository;
		private readonly ILogger<CreateAdvocateInvoiceConsumer> _logger;

		public CreateAdvocateInvoiceConsumer(
			SolvDbContext dbContext,
			IAdvocateInvoiceRepository advocateInvoiceRepository,
			IInvoicingCycleRepository invoicingCycleRepository,
			IAdvocateRepository advocateRepository,
			IBillingDetailsRepository billingDetailsRepository,
			ITicketRepository ticketRepository,
			ISequenceRepository sequenceRepository,
			ILogger<CreateAdvocateInvoiceConsumer> logger)
		{
			_invoicingCycleRepository = invoicingCycleRepository;
			_advocateRepository = advocateRepository;
			_billingDetailsRepository = billingDetailsRepository;
			_ticketRepository = ticketRepository;
			_sequenceRepository = sequenceRepository;
			_dbContext = dbContext;
			_advocateInvoiceRepository = advocateInvoiceRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<CreateAdvocateInvoiceCommand> context)
		{
			if (context.Message.IsValid())
			{
				_logger.LogDebug("Creating advocate invoice for {advocateId} for cycle {invoicingCycleId}", context.Message.AdvocateId, context.Message.InvoicingCycleId);

				// fetch basic invoice data
				var invoicingCycle = await _invoicingCycleRepository.FindAsync(context.Message.InvoicingCycleId);
				var (from, to) = (invoicingCycle.From, invoicingCycle.To);
				from = from.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week
				var ticketsByBrand = await _ticketRepository.GetTicketsForAdvocateInvoice(from, to, context.Message.AdvocateId);
				var platformBillingDetailsId = await _billingDetailsRepository.GetCurrentIdForPlatform();
				var brands = await _advocateRepository.GetBrandIdsForInvoicing(context.Message.AdvocateId);

				// create invoice lines for the brands that either:
				// a) advocate has close the tickets for in the period
				// b) advocate is authorized to solve tickets right now
				var ticketLines = ticketsByBrand.Select(b => new AdvocateInvoiceLineItem(b.brandId, b.priceTotal, b.ticketsCount));
				var brandLines = brands.Select(b => new AdvocateInvoiceLineItem(b, 0m, 0));
				var lineItems = ticketLines.Union(brandLines)
					.GroupBy(x => x.BrandId)
					.Select(x => new AdvocateInvoiceLineItem(x.Key, x.Sum(y => y.Amount), x.Sum(y => y.TicketsCount)))
					.ToList();

				using (var t = await _dbContext.Database.BeginTransactionAsync())
				{
					// fetch sequence number for new invoice
					var advocateSequenceName = $"{nameof(AdvocateInvoice)}-{context.Message.AdvocateId}";
					var sequence = await _sequenceRepository.Next(advocateSequenceName);

					// create invoice
					var invoice = new AdvocateInvoice(invoicingCycle.Id, platformBillingDetailsId, context.Message.AdvocateId, sequence, lineItems.ToList());
					await _advocateInvoiceRepository.InsertAsync(invoice);

					// save new invoice
					await _advocateInvoiceRepository.SaveChangesAsync();

					// relate tickets involved in calculations with created invoice
					await _ticketRepository.SetTicketsAdvocateInvoiceId(invoice.Id, invoice.AdvocateId, from, to);
					await t.CommitAsync();
				}

				_logger.LogDebug("Invoice for advocate {advocateId} created", context.Message.AdvocateId);
			}
			else
			{
				_logger.LogError("Validation failed for {message}", context.Message);
			}
		}
	}
}