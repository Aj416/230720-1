using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tigerspike.Solv.Core.Configuration;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Messaging.Invoicing;
using Tigerspike.Solv.Services.Invoicing.Application.Commands;
using Tigerspike.Solv.Services.Invoicing.Enums;
using Tigerspike.Solv.Services.Invoicing.Infrastructure.Interfaces;
using Tigerspike.Solv.Services.Invoicing.Models;

namespace Tigerspike.Solv.Services.Invoicing.Application.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly InvoicingOptions _invoicingOptions;
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediatorHandler;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;
		private readonly ILogger<InvoiceService> _logger;
		private readonly IRequestClient<IBrandInfoCommand> _brandClient;
		private readonly IRequestClient<IFetchAdvocateInfoCommand> _advocateClient;
		private readonly IRequestClient<IFetchTicketInfoCommand> _ticketClient;

		public InvoiceService(
			IOptions<InvoicingOptions> invoicingOptions,
			IMapper mapper,
			IMediatorHandler mediatorHandler,
			IRequestClient<IBrandInfoCommand> brandClient,
			IRequestClient<IFetchAdvocateInfoCommand> advocateClient,
			IRequestClient<IFetchTicketInfoCommand> ticketClient,
			IInvoicingCycleRepository invoicingCycleRepository,
			IClientInvoiceRepository clientInvoiceRepository,
			IAdvocateInvoiceRepository advocateInvoiceRepository,
			ILogger<InvoiceService> logger)
		{
			_invoicingOptions = invoicingOptions?.Value;
			_mapper = mapper;
			_mediatorHandler = mediatorHandler;
			_brandClient = brandClient;
			_advocateClient = advocateClient;
			_ticketClient = ticketClient;
			_invoicingCycleRepository = invoicingCycleRepository;
			_clientInvoiceRepository = clientInvoiceRepository;
			_advocateInvoiceRepository = advocateInvoiceRepository;
			_logger = logger;
		}

		public async Task CreateBillingDetails(Guid brandId, BillingDetailsModel billingDetailsModel)
		{
			var result =
				await _brandClient.GetResponse<IBrandValidationResult>(
					new BrandInfoCommand(brandId));

			if (result.Message.IsSuccess)
			{
				await _mediatorHandler.SendCommand(new CreateBillingDetailsCommand(brandId, billingDetailsModel.Name, billingDetailsModel.Email, billingDetailsModel.VatNumber, billingDetailsModel.CompanyNumber, billingDetailsModel.Address, false));
			}
			else
			{
				await _mediatorHandler.RaiseEvent(new DomainNotification(GetType().Name, $"Brand with Id {brandId} not found."));
			}
		}

		public async Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid invoicingCycleId, PagedRequestModel pageRequest, SortOrder sortOrder, InvoiceSortBy sortBy)
		{
			var page = await _advocateInvoiceRepository.GetPagedListAsync(
				predicate: x => x.InvoicingCycleId == invoicingCycleId && x.Total > 0,
				orderBy: x => x.OrderBy(sortBy, sortOrder),
				pageIndex: pageRequest.PageIndex,
				pageSize: pageRequest.PageSize
			);

			var result = PagedList.From(page, _mapper.Map<IEnumerable<AdvocateInvoiceModel>>);

			var fetchAdvocateInfoResult =
				await _advocateClient.GetResponse<IFetchAdvocateInfoResult>(
					new FetchAdvocateInfoCommand(page.Items.Select(x => x.AdvocateId)));

			if (fetchAdvocateInfoResult.Message.IsSuccess)
			{
				var msg = fetchAdvocateInfoResult.Message;

				var finalResult = from l1 in result.Items
								  join l2 in msg.AdvocateInfo
									on l1.AdvocateId equals l2.AdvocateId
								  select new AdvocateInvoiceModel($"{l2.FirstName} {l2.LastName}", l2.AdvocateStatus, l1.Id, l1.ReferenceNumber, l1.CreatedDate, l1.Total,
								  l1.AdvocateId, l1.Status, l1.PaymentFailureDate);

				return PagedList.FromExisting(finalResult.ToList(), pageRequest.PageIndex, pageRequest.PageSize, page.TotalCount, 0);
			}

			return result;
		}

		public async Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid advocateId)
		{
			var page = await _advocateInvoiceRepository.GetPagedListAsync(
				predicate: x => x.AdvocateId == advocateId,
				orderBy: x => x.OrderByDescending(y => y.CreatedDate),
				pageSize: 5
			);

			var result = PagedList.From(page, _mapper.Map<IEnumerable<AdvocateInvoiceModel>>);

			var fetchAdvocateInfoResult =
				await _advocateClient.GetResponse<IFetchAdvocateInfoResult>(
					new FetchAdvocateInfoCommand(new[] { advocateId }));

			if (fetchAdvocateInfoResult.Message.IsSuccess)
			{
				var msg = fetchAdvocateInfoResult.Message;

				var finalResult = from l1 in result.Items
								  join l2 in msg.AdvocateInfo
									on l1.AdvocateId equals l2.AdvocateId
								  select new AdvocateInvoiceModel($"{l2.FirstName} {l2.LastName}", l2.AdvocateStatus, l1.Id, l1.ReferenceNumber, l1.CreatedDate, l1.Total,
								  l1.AdvocateId, l1.Status, l1.PaymentFailureDate);

				return PagedList.FromExisting(finalResult.ToList(), 0, 5, page.TotalCount, 0);
			}


			return result;
		}

		public async Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId)
		{
			var invoice = await _advocateInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId,
				include: x => x
				.Include(inc => inc.InvoicingCycle)
				.Include(inc => inc.PlatformBillingDetails)
				.Include(inc => inc.LineItems)
			);

			if (invoice == null)
			{
				await _mediatorHandler.RaiseEvent(new DomainNotification(GetType().Name, $"Advocate invoice with invoice id {invoiceId} not found."));
				return null;
			}

			var result = _mapper.Map<AdvocateInvoicePrintableModel>(invoice);

			var fetchAdvocateInfoResult =
				await _advocateClient.GetResponse<IFetchAdvocateInfoResult>(
					new FetchAdvocateInfoCommand(new[] { invoice.AdvocateId }));

			if (fetchAdvocateInfoResult.Message.IsSuccess)
			{
				var advocateInfo = fetchAdvocateInfoResult.Message.AdvocateInfo.FirstOrDefault();
				result.Advocate = new AdvocatePrintableModel
				{
					FirstName = advocateInfo.FirstName,
					LastName = advocateInfo.LastName,
					Phone = advocateInfo.Phone,
					Email = advocateInfo.Email,
					CountryCode = advocateInfo.CountryCode
				};
			}

			var fetchTicketInfoResult =
				await _ticketClient.GetResponse<IFetchTicketInfoResult>(
					new FetchTicketInfoCommand(null, invoiceId));

			if (fetchTicketInfoResult.Message.IsSuccess)
			{
				var msg = fetchTicketInfoResult.Message;
				result.Tickets = msg.TicketInfo.Select(t => new TicketPrintableModel
				{
					Id = t.Id,
					CreatedDate = t.CreatedDate,
					Fee = t.Fee,
					Price = t.Price,
					Total = t.Total,
					Brand = new BrandPrintableModel
					{
						Id = t.Brand.Id,
						Name = t.Brand.Name
					}
				});
			}

			result.Tickets = result.Tickets.OrderBy(x => x.CreatedDate).ToList();
			result.LineItems = result.LineItems.OrderByDescending(x => x.TicketsCount).ToList();
			return result;
		}

		public async Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId, Guid advocateId)
		{
			var invoice = await _advocateInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId && x.AdvocateId == advocateId,
				include: x => x
				.Include(inc => inc.InvoicingCycle)
				.Include(inc => inc.PlatformBillingDetails)
				.Include(inc => inc.LineItems)
			);

			if (invoice == null)
			{
				await _mediatorHandler.RaiseEvent(new DomainNotification(GetType().Name, $"Advocate invoice for {advocateId} with invoice id {invoiceId} not found."));
				return null;
			}

			var result = _mapper.Map<AdvocateInvoicePrintableModel>(invoice);

			var fetchAdvocateInfoResult =
				await _advocateClient.GetResponse<IFetchAdvocateInfoResult>(
					new FetchAdvocateInfoCommand(new[] { invoice.AdvocateId }));

			if (fetchAdvocateInfoResult.Message.IsSuccess)
			{
				var advocateInfo = fetchAdvocateInfoResult.Message.AdvocateInfo.FirstOrDefault();
				result.Advocate = new AdvocatePrintableModel
				{
					FirstName = advocateInfo.FirstName,
					LastName = advocateInfo.LastName,
					Phone = advocateInfo.Phone,
					Email = advocateInfo.Email,
					CountryCode = advocateInfo.CountryCode
				};
			}

			var fetchTicketInfoResult =
				await _ticketClient.GetResponse<IFetchTicketInfoResult>(
					new FetchTicketInfoCommand(null, invoiceId));

			if (fetchTicketInfoResult.Message.IsSuccess)
			{
				var msg = fetchTicketInfoResult.Message;
				result.Tickets = msg.TicketInfo.Select(t => new TicketPrintableModel
				{
					Id = t.Id,
					CreatedDate = t.CreatedDate,
					Fee = t.Fee,
					Price = t.Price,
					Total = t.Total,
					Brand = new BrandPrintableModel
					{
						Id = t.Brand.Id,
						Name = t.Brand.Name
					}
				});
			}

			result.Tickets = result.Tickets.OrderBy(x => x.CreatedDate).ToList();
			result.LineItems = result.LineItems.OrderByDescending(x => x.TicketsCount).ToList();
			return result;
		}

		public async Task<IPagedList<ClientInvoiceModel>> GetClientInvoiceList(Guid brandId, PagedRequestModel pageRequest, SortOrder sortOrder, InvoiceSortBy sortBy)
		{
			var page = await _clientInvoiceRepository.GetPagedListAsync(
				predicate: x => x.BrandId == brandId,
				orderBy: x => x.OrderBy(sortBy, sortOrder),
				pageIndex: pageRequest.PageIndex,
				pageSize: pageRequest.PageSize
			);

			return PagedList.From(page, _mapper.Map<IEnumerable<ClientInvoiceModel>>);
		}

		public async Task<ClientInvoicePrintableModel> GetClientInvoicePrintable(Guid invoiceId, Guid brandId)
		{
			var invoice = await _clientInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId && x.BrandId == brandId,
				include: x => x
					.Include(inc => inc.InvoicingCycle)
					.Include(inc => inc.BrandBillingDetails)
					.Include(inc => inc.PlatformBillingDetails)
			);

			if (invoice == null)
			{
				await _mediatorHandler.RaiseEvent(new DomainNotification(GetType().Name, $"Client invoice for {brandId} with invoice id {invoiceId} not found."));
				return null;
			}

			var model = _mapper.Map<ClientInvoicePrintableModel>(invoice);

			var brandValidationResult =
				await _brandClient.GetResponse<IBrandValidationResult>(
					new BrandInfoCommand(brandId));

			if (brandValidationResult.Message.IsSuccess)
			{
				model.PaymentRouteName = brandValidationResult.Message.PaymentRouteName;
			}

			var fetchTicketInfoResult =
				await _ticketClient.GetResponse<IFetchTicketInfoResult>(
					new FetchTicketInfoCommand(invoiceId, null));

			if (fetchTicketInfoResult.Message.IsSuccess)
			{
				var msg = fetchTicketInfoResult.Message;

				model.Tickets = msg.TicketInfo.Select(t => new TicketPrintableModel
				{
					Id = t.Id,
					CreatedDate = t.CreatedDate,
					Fee = t.Fee,
					Price = t.Price,
					Total = t.Total,
					Brand = new BrandPrintableModel
					{
						Id = t.Brand.Id,
						Name = t.Brand.Name
					}
				});
			}

			return model;
		}

		public async Task<Guid?> GetLastInvoicingCycleId() => await _invoicingCycleRepository.GetFirstOrDefaultAsync(selector: x => x.Id, orderBy: x => x.OrderByDescending(y => y.To));

		public async Task PayAdvocateInvoice(Guid advocateInvoiceId) => await _mediatorHandler.SendCommand(new PayAdvocateInvoiceCommand(advocateInvoiceId));

		public async Task PayClientInvoice(Guid clientInvoiceId) => await _mediatorHandler.SendCommand(new PayClientInvoiceCommand(clientInvoiceId));

		/// <inheritdoc />
		public async Task<StartInvoicingCycleResult> StartInvocingCyle(DateTime date)
		{
			_logger.LogInformation("StartInvoicingCycleCommand received successfully at {0}", DateTime.UtcNow);

			var periodicity = _invoicingOptions.Periodicity;
			if (periodicity != Periodicity.Weekly)
			{
				throw new InvalidOperationException("Only weekly invoice generating is supported currently");
			}

			// Generate weekly period
			var invoicingCycleStartDate = date.Date.StartOfWeek(DayOfWeek.Monday);
			var invoicingCycleEndDate = invoicingCycleStartDate.AddDays(7);

			// Check the current cycle is closed and no invoicing cycle has been created already
			DateTime? lastInvoicingCycleEndDate = await _invoicingCycleRepository.GetFirstOrDefaultAsync(s => s.To, orderBy: i => i.OrderByDescending(s => s.To));

			if (lastInvoicingCycleEndDate != null && invoicingCycleStartDate < lastInvoicingCycleEndDate)
			{
				throw new InvalidOperationException($"Validation failed for {GetType().Name}. The invoicing cycle date is not valid.");
			}

			var invoicingId = await _mediatorHandler.SendCommand(new CreateInvoicingCycleCommand(invoicingCycleStartDate, invoicingCycleEndDate));

			return new StartInvoicingCycleResult
			{
				Success = invoicingId != null,
				Id = invoicingId ?? Guid.Empty
			};
		}
	}
}
