using System;
using System.Threading.Tasks;
using Refit;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IInvoicingServiceClient
	{
		/// <summary>
		/// Create Billing Details
		/// </summary>
		/// <param name="brandId">Brand identifier.</param>
		/// <param name="model">Model of type BillingDetailsModel.</param>
		/// <returns></returns>
		[Post("/{brandId}/billing-details")]
		[Headers("Content-Type:application/json")]
		Task CreateBillingDetails([Query] Guid brandId, [Body] BillingDetailsModel model);

		/// <summary>
		/// Returns invoice data
		/// </summary>
		/// <param name="invoiceId">Invoice identifier.</param>
		/// <returns></returns>
		[Get("/advocate-invoices/{invoiceId}")]
		[Headers("Content-Type:application/json")]
		Task<AdvocateInvoicePrintableModel> GetAdvocateInvoice([Query] Guid invoiceId);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="advocateId">Advocate Identifier.</param>
		/// <param name="invoiceId">Invoice Identifier.</param>
		/// <returns></returns>
		[Get("/{advocateId}/advocate-invoices/{invoiceId}")]
		[Headers("Content-Type:application/json")]
		Task<AdvocateInvoicePrintableModel> GetAdvocateInvoice([Query] Guid advocateId, [Query] Guid invoiceId);

		/// <summary>
		/// Returns list of payments of the advocate
		/// </summary>
		/// <param name="advocateId">Advocate Identifier.</param>
		/// <returns></returns>
		[Get("/{advocateId}/advocate-invoices")]
		[Headers("Content-Type:application/json")]
		Task<PagedList<AdvocateInvoiceModel>> GetAdvocateInvoices([Query] Guid advocateId);

		/// <summary>
		/// Returns invoice data.
		/// </summary>
		/// <param name="brandId">Brand Identifier.</param>
		/// <param name="invoiceId">Invoice Identifier.</param>
		/// <returns></returns>
		[Get("/{brandId}/invoices/{invoiceId}")]
		[Headers("Content-Type:application/json")]
		Task<ClientInvoicePrintableModel> GetBrandInvoice([Query] Guid brandId, [Query] Guid invoiceId);

		/// <summary>
		/// Returns list of brand invoices (paged)
		/// </summary>
		/// <param name="brandId">Brand Identifier.</param>
		/// <param name="pageRequest">Enum of type PagedRequestModel.</param>
		/// <param name="sortOrder">Enum of type SortOrder.</param>
		/// <param name="sortBy">Enum of type InvoiceSortBy.</param>
		/// <returns></returns>
		[Get("/{brandId}/brand-invoices")]
		[Headers("Content-Type:application/json")]
		Task<PagedList<ClientInvoiceModel>> GetBrandInvoices([Query] Guid brandId, [Query] PagedRequestModel pageRequest, [Query] SortOrder sortOrder, [Query] InvoiceSortBy sortBy);

		/// <summary>
		/// Returns advocate invoices
		/// </summary>
		/// <param name="pageRequest">Enum of type PagedRequestModel.</param>
		/// <param name="sortOrder">Enum of type SortOrder.</param>
		/// <param name="sortBy">Enum of type InvoiceSortBy.</param>
		/// <returns></returns>
		[Get("/advocates")]
		[Headers("Content-Type:application/json")]
		Task<PagedList<AdvocateInvoiceModel>> GetCurrentAdvocateInvoices([Query] PagedRequestModel pageRequest, [Query] SortOrder sortOrder, [Query] InvoiceSortBy sortBy);

		/// <summary>
		/// Execute a set of payments from brands to advocates (total sum of prices for solved tickets) based on a AdvocateInvoice
		/// </summary>
		/// <param name="advocateInvoiceId">AdvocateInvoice Identifier.</param>
		/// <returns></returns>
		[Post("/pay/advocate/{advocateInvoiceId}")]
		[Headers("Content-Type:application/json")]
		Task PayAdvocateInvoice([Query] Guid advocateInvoiceId);

		/// <summary>
		/// Execute a payment from brand to platform (fees + VAT) based on a ClientInvoice.
		/// </summary>
		/// <param name="clientInvoiceId">ClientInvoice Identifier.</param>
		/// <returns></returns>
		[Post("/pay/platform/{clientInvoiceId}")]
		[Headers("Content-Type:application/json")]
		Task PayClientInvoice([Query] Guid clientInvoiceId);

		/// <summary>
		/// Schedules recurring invoicing cycles.
		/// </summary>
		/// <param name="state">State of cycle.</param>
		/// <returns></returns>
		[Get("/recurring-invoicing-cycle/{state}")]
		[Headers("Content-Type:application/json")]
		Task<RecurringInvoicingCycleResultModel> RecurringInvoicingCycle([Query] bool state);

		/// <summary>
		/// Schedules invoice generation.
		/// </summary>
		/// <param name="startdate">Date from which to start invocing.</param>
		/// <returns></returns>
		[Get("/start-invoicing-cycle")]
		[Headers("Content-Type:application/json")]
		Task<StartInvoicingCycleResultModel> StartInvoicingCycle([Query] DateTime? startdate);
	}
}
