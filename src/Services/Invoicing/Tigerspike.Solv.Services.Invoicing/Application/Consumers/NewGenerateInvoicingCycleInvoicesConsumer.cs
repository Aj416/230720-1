using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Application.Consumers
{
	public class NewGenerateInvoicingCycleInvoicesConsumer :
		IConsumer<IGenerateInvoicingCycleInvoicesCommand>
	{
		private readonly IRequestClient<IFetchAdvocateIdsForInvoicingCommand> _advocateClient;
		private readonly IRequestClient<IBrandIdForInvoicingCommand> _brandClient;
		private readonly IRequestClient<IFetchAdvocatesToInvoiceCommand> _ticketClient;
		private readonly ILogger<NewGenerateInvoicingCycleInvoicesConsumer> _logger;

		public NewGenerateInvoicingCycleInvoicesConsumer(
			IRequestClient<IFetchAdvocateIdsForInvoicingCommand> advocateClient,
			IRequestClient<IBrandIdForInvoicingCommand> brandClient,
			IRequestClient<IFetchAdvocatesToInvoiceCommand> ticketClient,
			ILogger<NewGenerateInvoicingCycleInvoicesConsumer> logger)
		{
			_advocateClient = advocateClient;
			_brandClient = brandClient;
			_ticketClient = ticketClient;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IGenerateInvoicingCycleInvoicesCommand> context)
		{

			var msg = context.Message;

			_logger.LogDebug("Started consuming invoicing cycle {invoicingCycleId}", msg.InvoicingCycleId);

			var invoicingBrandIdResult =
				await _brandClient.GetResponse<IInvoicingBrandIdResult>(
					new BrandIdForInvoicingCommand(true));

			_logger.LogDebug($"Scheduling creation of {invoicingBrandIdResult.Message.BrandIds.Count()} client invoices");

			foreach (var brandId in invoicingBrandIdResult.Message.BrandIds)
			{
				await context.Send<ICreateClientInvoiceCommand>(
					new
					{
						msg.InvoicingCycleId,
						BrandId = brandId
					});
			}

			_logger.LogDebug("Started consuming invoicing cycle {invoicingCycleId}", msg.InvoicingCycleId);

			var from = msg.From.AddDays(-7); // due to https://tigerspike.atlassian.net/browse/DCTXS2-3036 consider invoicing also uninvoiced tickets from the last week
			var to = msg.To;

			var fetchAdvocateIdsForInvoicingResult =
				await _advocateClient.GetResponse<IFetchAdvocateIdsForInvoicingResult>(
					new FetchAdvocateIdsForInvoicingCommand(true, true));

			var fetchAdvocatesToInvoiceResult =
				await _ticketClient.GetResponse<IFetchAdvocateIdsForInvoicingResult>(
					new FetchAdvocatesToInvoiceCommand(from, to));

			var advocatesToInvoices = Enumerable.Union(fetchAdvocateIdsForInvoicingResult.Message.AdvocateIds, fetchAdvocatesToInvoiceResult.Message.AdvocateIds);

			_logger.LogDebug("Scheduling creation of {count} advocate invoices", advocatesToInvoices.Count());

			foreach (var advocatesToInvoice in advocatesToInvoices)
			{
				await context.Send<ICreateAdvocateInvoiceCommand>(
					new
					{
						AdvocateId = advocatesToInvoice,
						msg.InvoicingCycleId,
					});
			}
		}
	}
}
