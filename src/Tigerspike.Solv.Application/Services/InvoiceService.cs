using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Commands.Invoice;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IMapper _mapper;
		private readonly IMediatorHandler _mediatorHandler;
		private readonly IInvoicingCycleRepository _invoicingCycleRepository;
		private readonly IClientInvoiceRepository _clientInvoiceRepository;
		private readonly IAdvocateInvoiceRepository _advocateInvoiceRepository;

		public InvoiceService(
			IMapper mapper,
			IMediatorHandler mediatorHandler,
			IInvoicingCycleRepository invoicingCycleRepository,
			IClientInvoiceRepository clientInvoiceRepository,
			IAdvocateInvoiceRepository advocateInvoiceRepository)
		{
			_mapper = mapper;
			_mediatorHandler = mediatorHandler;
			_invoicingCycleRepository = invoicingCycleRepository;
			_clientInvoiceRepository = clientInvoiceRepository;
			_advocateInvoiceRepository = advocateInvoiceRepository;
		}

		/// <inheritdoc/>
		public async Task<ClientInvoicePrintableModel> GetClientInvoicePrintable(Guid invoiceId, Guid brandId)
		{
			var invoice = await _clientInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId && x.BrandId == brandId,
				include: x => x
					.Include(inc => inc.InvoicingCycle)
					.Include(inc => inc.BrandBillingDetails)
					.Include(inc => inc.PlatformBillingDetails)
					.Include(inc => inc.Tickets)
					.Include(inc => inc.Brand).ThenInclude(tinc => tinc.PaymentRoute)
			);

			return _mapper.Map<ClientInvoicePrintableModel>(invoice);
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public async Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid invoicingCycleId, PagedRequestModel pageRequest, SortOrder sortOrder, InvoiceSortBy sortBy)
		{
			var page = await _advocateInvoiceRepository.GetPagedListAsync(
				predicate: x => x.InvoicingCycleId == invoicingCycleId && x.Total > 0,
				orderBy: x => x.OrderBy(sortBy, sortOrder),
				pageIndex: pageRequest.PageIndex,
				pageSize: pageRequest.PageSize,
				include: x => x
					.Include(inc => inc.Advocate).ThenInclude(inc => inc.User)
					.Include(inc => inc.LineItems).ThenInclude(inc => inc.Payment)
			);

			return PagedList.From(page, _mapper.Map<IEnumerable<AdvocateInvoiceModel>>);
		}

		/// <inheritdoc/>
		public async Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid advocateId)
		{
			var page = await _advocateInvoiceRepository.GetPagedListAsync(
				predicate: x => x.AdvocateId == advocateId,
				orderBy: x => x.OrderByDescending(y => y.CreatedDate),
				pageSize: 5
			);

			return PagedList.From(page, _mapper.Map<IEnumerable<AdvocateInvoiceModel>>);
		}

		/// <inheritdoc/>
		public async Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId, Guid advocateId)
		{
			var invoice = await _advocateInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId && x.AdvocateId == advocateId,
				include: x => x
				.Include(inc => inc.InvoicingCycle)
				.Include(inc => inc.PlatformBillingDetails)
				.Include(inc => inc.Tickets).ThenInclude(inc => inc.Brand)
				.Include(inc => inc.Advocate).ThenInclude(inc => inc.User)
				.Include(inc => inc.LineItems).ThenInclude(inc => inc.Brand)
			);

			var result = _mapper.Map<AdvocateInvoicePrintableModel>(invoice);
			result.Tickets = result.Tickets.OrderBy(x => x.CreatedDate).ToList();
			result.LineItems = result.LineItems.OrderByDescending(x => x.TicketsCount).ToList();
			return result;
		}

		/// <inheritdoc/>
		public async Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId)
		{
			var invoice = await _advocateInvoiceRepository.GetSingleOrDefaultAsync(
				predicate: x => x.Id == invoiceId,
				include: x => x
				.Include(inc => inc.InvoicingCycle)
				.Include(inc => inc.PlatformBillingDetails)
				.Include(inc => inc.Tickets).ThenInclude(inc => inc.Brand)
				.Include(inc => inc.Advocate).ThenInclude(inc => inc.User)
				.Include(inc => inc.LineItems).ThenInclude(inc => inc.Brand)
			);

			var result = _mapper.Map<AdvocateInvoicePrintableModel>(invoice);
			result.Tickets = result.Tickets.OrderBy(x => x.CreatedDate).ToList();
			result.LineItems = result.LineItems.OrderByDescending(x => x.TicketsCount).ToList();
			return result;
		}

		/// <inheritdoc/>
		public async Task<Guid?> GetLastInvoicingCycleId() => await _invoicingCycleRepository.GetFirstOrDefaultAsync(selector: x => x.Id, orderBy: x => x.OrderByDescending(y => y.To));

		/// <inheritdoc/>
		public async Task PayAdvocateInvoice(Guid advocateInvoiceId) => await _mediatorHandler.SendCommand(new PayAdvocateInvoiceCommand(advocateInvoiceId));

		/// <inheritdoc/>
		public async Task PayClientInvoice(Guid clientInvoiceId) => await _mediatorHandler.SendCommand(new PayClientInvoiceCommand(clientInvoiceId));

		/// <inheritdoc/>
		public async Task CreateBillingDetails(Guid brandId, BillingDetailsModel billingDetailsModel) => await _mediatorHandler.SendCommand(new CreateBillingDetailsCommand(brandId, billingDetailsModel.Name, billingDetailsModel.Email, billingDetailsModel.VatNumber, billingDetailsModel.CompanyNumber, billingDetailsModel.Address, false));
	}
}