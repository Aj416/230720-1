using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IInvoiceService
	{

		/// <summary>
		/// Gets printable model of clients invoice
		/// </summary>
		/// <param name="invoiceId">The invoice id</param>
		/// <param name="brandId">The brand id - for verification purposes</param>
		Task<ClientInvoicePrintableModel> GetClientInvoicePrintable(Guid invoiceId, Guid brandId);

		/// <summary>
		/// Return paged listing for client invoices
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="pageRequest">The page request (index, size)</param>
		/// <param name="sortOrder">Sorting order</param>
		/// <param name="sortBy">Sorting field</param>
		/// <returns>One page of results</returns>
		Task<IPagedList<ClientInvoiceModel>> GetClientInvoiceList(Guid brandId, PagedRequestModel pageRequest, SortOrder sortOrder, InvoiceSortBy sortBy);

		/// <summary>
		/// Returns advocate invoices for the given invoicing cycle
		/// </summary>
		/// <param name="invoicingCycleId">The invoicing cycle id</param>
		/// <param name="pageRequest">The page request (index, size)</param>
		/// <param name="sortOrder">Sorting order</param>
		/// <param name="sortBy">Sorting field</param>
		/// <returns>One page of results</returns>
		Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid invoicingCycleId, PagedRequestModel pageRequest, SortOrder sortOrder, InvoiceSortBy sortBy);

		/// <summary>
		/// Get advocate recent invoices list
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <returns>Returns last 5 invoices and information how many there are in total</returns>
		Task<IPagedList<AdvocateInvoiceModel>> GetAdvocateInvoiceList(Guid advocateId);

		/// <summary>
		/// Get printable model of advocates invoice
		/// </summary>
		/// <param name="invoiceId">The invoice id</param>
		Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId);

		/// <summary>
		/// Get printable model of advocates invoice
		/// </summary>
		/// <param name="invoiceId">The invoice id</param>
		/// /// <param name="advocateId">The advocate id</param>
		Task<AdvocateInvoicePrintableModel> GetAdvocateInvoicePrintable(Guid invoiceId, Guid advocateId);

		/// <summary>
		/// Pay the advocate invoice (brand -> advocate).
		/// </summary>
		/// <param name="advocateInvoiceId">The advocate invoice id to be paid</param>
		Task PayAdvocateInvoice(Guid advocateInvoiceId);

		/// <summary>
		/// Pay the client invoice (brand -> platform).
		/// </summary>
		/// <param name="clientInvoiceId">The client invoice id to be paid</param>
		Task PayClientInvoice(Guid clientInvoiceId);

		/// <summary>
		/// Returns last invoicing cycle id in the platform
		/// </summary>
		Task<Guid?> GetLastInvoicingCycleId();

		/// <summary>
		/// Create new Billing Details for brand
		/// </summary>
		Task CreateBillingDetails(Guid brandId, BillingDetailsModel billingDetailsModel);
	}
}