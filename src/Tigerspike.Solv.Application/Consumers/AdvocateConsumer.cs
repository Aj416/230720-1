using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Messaging.IdentityVerification;
using Tigerspike.Solv.Messaging.Invoicing;

namespace Tigerspike.Solv.Application.Consumers
{
	public class AdvocateConsumer :
		IConsumer<IFetchAdvocateIdsForInvoicingCommand>,
		IConsumer<IFetchBrandIdsForInvoicingCommand>,
		IConsumer<IFetchAdvocateInfoCommand>,
		IConsumer<IFetchAdvocateDetailsCommand>
	{
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IMediatorHandler _mediator;
		private readonly ILogger<AdvocateConsumer> _logger;

		public AdvocateConsumer(
			IAdvocateRepository advocateRepository,
			IMediatorHandler mediator,
			ILogger<AdvocateConsumer> logger)
		{
			_advocateRepository = advocateRepository;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IFetchAdvocateIdsForInvoicingCommand> context)
		{

			_logger.LogDebug("Consuming IFetchAdvocateIdsForInvoicingCommand to fetch advocate ids for invoicing");
			var advocatesWithAuthorisedBrands = await _advocateRepository.GetAllAdvocateIdsForInvoicing();

			await context.RespondAsync<IFetchAdvocateIdsForInvoicingResult>(new
			{
				IsSuccess = true,
				AdvocateIds = advocatesWithAuthorisedBrands
			});
		}

		public async Task Consume(ConsumeContext<IFetchBrandIdsForInvoicingCommand> context)
		{
			_logger.LogDebug("Consuming IFetchBrandIdsForInvoicingCommand to fetch brand ids for invoicing");

			var brands = await _advocateRepository.GetBrandDetailsForInvoicing(context.Message.AdvocateId);
			await context.RespondAsync<IFetchBrandIdsForInvoicingResult>(new
			{
				IsSuccess = true,
				BrandDetails = brands
			});
		}

		public async Task Consume(ConsumeContext<IFetchAdvocateInfoCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug("Consuming IFetchAdvocateInfoCommand to fetch advocate info");

			var advocates = await _advocateRepository.GetAllAsync(
				include: src => src.Include(a => a.User),
				predicate: a => msg.AdvocateIds.Contains(a.Id));

			await context.RespondAsync<IFetchAdvocateInfoResult>(new
			{
				IsSuccess = true,
				AdvocateInfo = advocates.Select(x => new
				{
					AdvocateId = x.Id,
					x.User.FirstName,
					x.User.LastName,
					x.User.Phone,
					x.User.Email,
					x.CountryCode,
					AdvocateStatus = (int)x.Status,
					x.PaymentAccountId
				})
			});
		}

		public async Task Consume(ConsumeContext<IFetchAdvocateDetailsCommand> context)
		{
			var msg = context.Message;
			_logger.LogDebug($"Consuming IFetchAdvocateDetailsCommand to fetch advocate {msg.AdvocateId} brand {msg.BrandId} info");

			var advocateDetails = await _advocateRepository.GetSingleOrDefaultAsync(
				selector: x => new
				{
					x.User.CreatedDate,
					x.User.Email,
					x.Brands.Single(b => b.BrandId == msg.BrandId).Csat,
					x.PaymentAccountId
				},
				predicate: x => x.Id == msg.AdvocateId
			);

			await context.RespondAsync<IFetchAdvocateDetailsResult>(new
			{
				IsSuccess = true,
				advocateDetails.CreatedDate,
				advocateDetails.Email,
				advocateDetails.Csat,
				advocateDetails.PaymentAccountId
			});
		}
	}
}
