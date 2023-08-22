using System;
using System.Threading.Tasks;
using Refit;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Application.Services
{
	public class InvoicingService : IInvoicingService
	{
		private readonly IInvoicingServiceClient _invoicingServiceClient;

		public InvoicingService(
			IInvoicingServiceClient invoicingServiceClient) => _invoicingServiceClient = invoicingServiceClient;

		/// <inheritdoc/>
		public Task CreateBillingDetails([Query] Guid brandId, [Body] BillingDetailsModel model) => _invoicingServiceClient.CreateBillingDetails(brandId, model);

		/// <inheritdoc/>
		public Task<AdvocateInvoicePrintableModel> GetAdvocateInvoice([Query] Guid invoiceId) => _invoicingServiceClient.GetAdvocateInvoice(invoiceId);

		/// <inheritdoc/>
		public Task<AdvocateInvoicePrintableModel> GetAdvocateInvoice([Query] Guid advocateId, [Query] Guid invoiceId) => _invoicingServiceClient.GetAdvocateInvoice(advocateId, invoiceId);

		/// <inheritdoc/>
		public Task<PagedList<AdvocateInvoiceModel>> GetAdvocateInvoices([Query] Guid advocateId) => _invoicingServiceClient.GetAdvocateInvoices(advocateId);

		/// <inheritdoc/>
		public Task<ClientInvoicePrintableModel> GetBrandInvoice([Query] Guid brandId, [Query] Guid invoiceId) => _invoicingServiceClient.GetBrandInvoice(brandId, invoiceId);

		/// <inheritdoc/>
		public Task<PagedList<ClientInvoiceModel>> GetBrandInvoices([Query] Guid brandId, [Query] PagedRequestModel pageRequest, [Query] SortOrder sortOrder, [Query] InvoiceSortBy sortBy)
			=> _invoicingServiceClient.GetBrandInvoices(brandId, pageRequest, sortOrder, sortBy);

		/// <inheritdoc/>
		public Task<PagedList<AdvocateInvoiceModel>> GetCurrentAdvocateInvoices([Query] PagedRequestModel pageRequest, [Query] SortOrder sortOrder, [Query] InvoiceSortBy sortBy)
			=> _invoicingServiceClient.GetCurrentAdvocateInvoices(pageRequest, sortOrder, sortBy);

		/// <inheritdoc/>
		public Task PayAdvocateInvoice([Query] Guid advocateInvoiceId) => _invoicingServiceClient.PayAdvocateInvoice(advocateInvoiceId);

		/// <inheritdoc/>
		public Task PayClientInvoice([Query] Guid clientInvoiceId) => _invoicingServiceClient.PayClientInvoice(clientInvoiceId);

		/// <inheritdoc/>
		public Task<RecurringInvoicingCycleResultModel> RecurringInvoicingCycle([Query] bool state) => _invoicingServiceClient.RecurringInvoicingCycle(state);

		/// <inheritdoc/>
		public Task<StartInvoicingCycleResultModel> StartInvoicingCycle([Query] DateTime? startdate) => _invoicingServiceClient.StartInvoicingCycle(startdate);
	}
}
