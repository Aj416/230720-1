using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Application.Consumers
{
	public class BrandConsumer :
		IConsumer<IBrandInfoCommand>,
		IConsumer<ISetBillingDetailsIdCommand>,
		IConsumer<IBrandIdForInvoicingCommand>,
		IConsumer<IFetchBrandBillingDetailCommand>
	{
		private readonly IBrandRepository _brandRepository;
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<BrandConsumer> _logger;

		public BrandConsumer(
			IBrandRepository brandRepository,
			IMediatorHandler mediator,
			ILogger<BrandConsumer> logger
			)
		{
			_brandRepository = brandRepository;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IBrandInfoCommand> context)
		{
			_logger.LogInformation($"Searching brand with id {context.Message.BrandId}");

			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				include: i => i.Include(b => b.PaymentRoute),
				predicate: b => b.Id == context.Message.BrandId);

			await context.RespondAsync<IBrandValidationResult>(new
			{
				PaymentRouteName = brand?.PaymentRoute?.Name,
				brand?.Name,
				brand?.BillingAgreementId,
				brand?.PaymentAccountId,
				brand?.CreatedDate,
				IsSuccess = brand != null
			});
		}

		public async Task Consume(ConsumeContext<ISetBillingDetailsIdCommand> context)
		{
			_logger.LogInformation($"Assign billing details id for brand {context.Message.BrandId}");

			await _mediator.SendCommand(new SetBrandBillingDetailsIdCommand(context.Message.BrandId, context.Message.BillingDetailsId));
		}

		public async Task Consume(ConsumeContext<IBrandIdForInvoicingCommand> context)
		{
			_logger.LogInformation($"Fetching brand id's for invoicing");
			var brandIds = await _brandRepository.GetBrandIdsForInvoicing();

			await context.RespondAsync<IInvoicingBrandIdResult>(new
			{
				IsSuccess = brandIds.Any(),
				BrandIds = brandIds
			});
		}

		public async Task Consume(ConsumeContext<IFetchBrandBillingDetailCommand> context)
		{
			_logger.LogInformation($"Fetching billing details for brand {context.Message.BrandId}");

			var brandDetails = await _brandRepository.GetFirstOrDefaultAsync(x => new { x.BillingDetailsId, x.VatRate }, x => x.Id == context.Message.BrandId);

			await context.RespondAsync<IFetchBrandBillingDetailResult>(new
			{
				IsSuccess = true,
				brandDetails.BillingDetailsId,
				brandDetails.VatRate
			});
		}
	}
}
