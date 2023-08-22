using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Core.Repositories;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces
{
	public interface ITicketRepository : IRepository<Ticket>
	{

		/// <summary>
		/// Gets the average time to respond for all tickets.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average time to respond in seconds, scoped to a given date range, if specified.</returns>
		Task<double?> GetAverageTimeToRespondForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the assigned tickets of an advocate with a selector
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <param name="selector">Selector expression</param>
		Task<IList<TResult>> GetAssignedTickets<TResult>(Guid advocateId, Expression<Func<Ticket, TResult>> selector);

		/// <summary>
		/// Gets the average time to respond for all tickets for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average time to respond in seconds, scoped to a given date range, if specified.</returns>
		Task<double?> GetAverageTimeToRespondForBrand(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average time to complete for closed tickets.
		/// </summary>
		/// <param name="advocateId">The advocate id. (optional)</param>
		/// <param name="brandId">The brand id. (optional)</param>
		/// <param name="from">Starting date (optional)</param>
		/// <param name="to">End date (optional)</param>
		/// <returns>The average time to complete in seconds, scoped to a given date range, if specified.</returns>
		Task<double?> GetAverageTimeToComplete(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the success rate from the first attempt.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The success rate, scoped to a given date range, if specified.</returns>
		Task<int?> GetFirstAttemptSuccessRateForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the success rate from the first attempt for a specific brand.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The success rate, scoped to a given date range, if specified.</returns>
		Task<int?> GetFirstAttemptSuccessRateForBrand(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average complexity of the closed tickets.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average complexity, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAverageComplexityForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average complexity of the closed tickets for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average complexity, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAverageComplexityForBrand(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the success rate of closed tickets.
		/// </summary>
		/// <param name="advocateId">The advocate id. (optional)</param>
		/// <param name="brandId">The brand id. (optional)</param>
		/// <param name="from">Starting date (optional)</param>
		/// <param name="to">End date (optional)</param>
		/// <returns>The success rate, scoped to a given date range, if specified.</returns>
		Task<int?> GetSuccessRate(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average csat of the closed tickets.
		/// </summary>
		/// <param name="advocateId">The advocate id. (optional)</param>
		/// <param name="brandId">The brand id. (optional)</param>
		/// <param name="from">Starting date (optional)</param>
		/// <param name="to">End date (optional)</param>
		/// <returns>The average csat, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAverageCsat(Guid? advocateId = null, Guid? brandId = null, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the total price of closed tickets.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The total price of closed tickets, scoped to a given date range, if specified.</returns>
		Task<decimal> GetTotalPriceForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the total price of closed tickets for specified brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The total price of closed tickets.</returns>
		Task<decimal> GetTotalPriceForBrand(Guid brandId);

		/// <summary>
		/// Gets the average price of the closed tickets for a specific brand in specified period.
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="fromDate">Left bound of the period (inclusive)</param>
		/// <param name="toDate">Right bound of the period (inclusive)</param>
		/// <returns>The total price of eligible tickets</returns>
		Task<decimal> GetTotalPriceForBrand(Guid brandId, DateTime fromDate, DateTime toDate);

		/// <summary>
		/// Return the ids' list of escalated tickets for a certain brand.
		/// </summary>
		Task<List<Guid>> GetEscalatedTicketIds(Guid brandId);

		/// <summary>
		/// Gets the total price of closed tickets for specified advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The total price of closed tickets, scoped to a given date range, if specified.</returns>
		Task<decimal> GetTotalPriceForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average price of the closed tickets.
		/// </summary>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average price, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAveragePriceForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average price of the closed tickets for a specific brand.
		/// </summary>
		/// <param name="brandId">The brand id.</param>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average price, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAveragePriceForBrand(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets the average price of the closed tickets for a specific advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="from">Optional starting date </param>
		/// <param name="to">Optional ending date</param>
		/// <returns>The average price, scoped to a given date range, if specified.</returns>
		Task<decimal?> GetAveragePriceForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Get the count of critical tickets for a specific brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <returns>The count of the critical tickets</returns>
		Task<int> GetCriticalCountForBrand(Guid brandId);

		/// <summary>
		/// Returns count of tickets in certain status for all tickets
		/// </summary>
		/// <returns>Dictionary where key is the ticket status and values is count of tickets in that status</returns>
		Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForAll();

		/// <summary>
		/// Returns count of tickets in certain status for brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <returns>Dictionary where key is the ticket status and values is count of tickets in that status for specified brand</returns>
		Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForBrand(Guid brandId);

		/// <summary>
		/// Returns count of tickets in certain status for advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <returns>Dictionary where key is the ticket status and values is count of tickets in that status for specified advocate</returns>
		Task<Dictionary<TicketStatusEnum, int>> GetCountByStatusForAdvocate(Guid advocateId);

		/// <summary>
		/// Returns the list of the ticket assigned to the advocate.
		/// </summary>
		Task<IEnumerable<Ticket>> GetAdvocateTickets(Guid advocateId);

		/// <summary>
		/// Returns the id of the ticket that still reserved for the advocate.
		/// Note: This is not a way to reserve a ticket, it is just to return the ticket that was reserved earlier.
		/// </summary>
		Task<Guid?> GetReservedTicketId(Guid advocateId);

		/// <summary>
		/// Return list of tickets which reservation has expired
		/// </summary>
		/// <returns>List of expired tickets (with StatusHistory and RejectionHistory included)</returns>
		Task<IList<Ticket>> GetExpiredTickets();

		/// <summary>
		/// Attempts to reserve a new ticket for the advocate with the specified id.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="advocateBrandIds">List of advocates active brands</param>
		/// <param name="level">Level of the ticket to look for.</param>
		/// <returns>True if a ticket has been reserved, false otherwise.</returns>
		Task<bool> ReserveTicket(Guid advocateId, IEnumerable<Guid> advocateBrandIds, TicketLevel level);

		/// <summary>
		/// Returns all the UI available rejection reasons.
		/// </summary>
		Task<IEnumerable<RejectionReason>> GetRejectionReasons();

		/// <summary>
		/// Checks if the passed array of reject reasons ids exist in the db.
		/// </summary>
		/// <param name="rejectReasonIds">The list of reject reasons ids to check</param>
		/// <returns>true/false</returns>
		Task<bool> RejectReasonsExist(int[] rejectReasonIds);

		/// <summary>
		/// Returns the full graph of a ticket, including all dependencies with tracking enabled.
		/// Mostly used for domain logic.
		/// </summary>
		/// <param name="predicate">The criteria to what ticket to bring</param>
		/// <returns>The full graph ticket</returns>
		Task<Ticket> GetFullTicket(Expression<Func<Ticket, bool>> predicate = null);

		/// <summary>
		/// Returns the ticket, including all tagging dependencies with tracking enabled.
		/// </summary>
		/// <param name="predicate">The criteria to what ticket to bring</param>
		/// <returns>The ticket with all tagging graphs</returns>
		Task<Ticket> GetTicketWithTaggingInfo(Expression<Func<Ticket, bool>> predicate = null);

		/// <summary>
		/// Gets practice ticked with advocateId
		/// </summary>
		/// <param name="advocateId"></param>
		/// <returns>Advocate Id</returns>
		Task<Guid?> GetPracticeTicket(Guid advocateId);

		/// <summary>
		/// Return the sum of all tickets price and fees in the specified invoicing cycle.
		/// </summary>
		/// <param name="invoicingCycleId">The invoice cycle id that tickets fall into</param>
		/// <returns>priceTotal is the sum of all ticket prices, feeTotal is the sum of all fees</returns>
		Task<(decimal priceTotal, decimal feeTotal, int ticketsCount)> GetClientInvoicingAmounts(DateTime fromDate, DateTime toDate, Guid brandId);

		/// <summary>
		/// Return a list of items, each item represent a brand and the number of tickets and total tickets price for that brand.
		/// All items belong to the specified advocate.
		/// </summary>
		/// <param name="invoicingCycleId">The invoice cycle id that tickets fall into</param>
		/// <returns>priceTotal is the sum of all ticket prices, ticketsCount is the number of tickets for each brand</returns>
		Task<List<(Guid brandId, decimal priceTotal, int ticketsCount)>> GetTicketsForAdvocateInvoice(DateTime fromDate, DateTime toDate, Guid advocateId);

		/// <summary>
		/// Associate the client invoice with the tickets that in the specified cycle and belong to the specified brand
		/// </summary>
		/// <returns>Number of tickets affected.</returns>
		Task<int> SetTicketsClientInvoiceId(Guid invoiceId, Guid brandId, DateTime fromDate, DateTime toDate);

		/// <summary>
		/// Associate the advocate invoice with the tickets that in the specified cycle and belong to the specified advocate
		/// </summary>
		/// <returns>Number of tickets affected.</returns>
		Task<int> SetTicketsAdvocateInvoiceId(Guid invoiceId, Guid advocateId, DateTime fromDate, DateTime toDate);

		/// <summary>
		/// Return the list of tickets id for a specific advocate invoice.
		/// </summary>
		/// <param name="advocateInvoiceId">The advocate invoice id</param>
		Task<List<(Guid id, Guid brandId, decimal price)>> GetTicketsInfoForAdvocateInvoice(Guid advocateInvoiceId);

		/// <summary>
		/// Return escalated tickets by source statistic
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="from">An optional starting date (must also then pass a value to the to parameter 'to', to work)</param>
		/// <param name="to">An optional ending date (will be used to create a date range, with the previous 'from' paramter)</param>
		Task<IList<(string source, int count)>> GetEscalatedBySource(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Returns brand id of the ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<Guid?> GetBrandId(Guid ticketId);

		/// <summary>
		/// Get closed tickets statistics for the advocate for specified period
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <param name="from">From timestamp</param>
		/// <param name="to">To timestamp</param>
		Task<(int closedTickets, decimal amount)> GetClosedStatisticForPeriod(Guid advocateId, DateTimeOffset? from, DateTime? to);

		/// <summary>
		/// Get paged ticket export data
		/// </summary>
		/// <param name="from">From timestamp</param>
		/// <param name="to">To timestamp</param>
		/// <param name="brandId">The brand id to filter by.</param>
		/// <param name="pageIndex">Page index to start with</param>
		/// <param name="pageSize">Page size</param>
		Task<IPagedList<Ticket>> GetPagedExportData(DateTime? from, DateTime? to, Guid? brandId, int pageIndex, int pageSize = 100);

		/// <summary>
		/// Returns the list of available ticket ids for the brands
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="level">The level of the tickets</param>
		Task<IEnumerable<Guid>> GetAvailableTicketIds(Guid brandId, TicketLevel level);

		/// <summary>
		/// Return list on all not-closed tickets touched by an advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<IEnumerable<Guid>> GetTouchedTicketIds(Guid advocateId);

		/// <summary>
		/// Get ticket tags
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		/// <param name="userLevel">The user level</param>
		Task<IEnumerable<Tag>> GetTags(Guid ticketId, TicketLevel userLevel);

		/// <summary>
		/// Return advocate performance breakdown for a given period.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="from">From timestamp.</param>
		/// <param name="to">To Timestamp.</param>
		/// <param name="brandIds">List of brand ids.</param>
		/// <param name="period">Period for getting statistics.</param>
		/// <returns>A list of objects, consisting of date, brandid and total price of tickets closed during the period.</returns>
		Task<IList<(DateTime key, Guid brandId, decimal totalPrice)>> GetAdvocatePerformanceBreakDownForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Period period);

		/// <summary>
		/// Return advocate performance summary for a given period.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="from">From timestamp.</param>
		/// <param name="to">To Timestamp.</param>
		/// <param name="brandIds">List of brand ids.</param>
		/// <returns>A list of objects, consisting of brandid, total price of tickets closed during the period and total count of closed tickets.</returns>
		Task<IList<(Guid brandId, decimal totalPrice, int totalCount)>> GetAdvocatePerformanceSummaryForPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds);

		/// <summary>
		/// Gets id of the ticket matching "Returning Customer" flow logic
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="email">The customer email</param>
		/// <param name="keyMetadata">The key metadata that have to match</param>
		Task<Guid?> GetReturningCustomerTicketId(Guid brandId, string email, IReadOnlyDictionary<string, string> keyMetadata);

		/// <summary>
		/// Gets the serial number for specified ticket.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <returns>The serial number of ticket.</returns>
		Task<string> GetSerialNumber(Guid ticketId);

		/// <summary>
		/// Returns date for event specific to ticket for a corresponding advocate.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <param name="advocateId">The advocate id.</param>
		/// <param name="status">Ticket status enum</param>
		/// <returns>Date for event specific to ticket for a corresponding advocate.</returns>
		Task<DateTime?> GetLastDateForTicketStatus(Guid ticketId, Guid advocateId, TicketStatusEnum status);

		/// <summary>
		/// Returns list of advocate to invoice in specified period
		/// </summary>
		/// <param name="fromDate">Left bound of the period (inclusive)</param>
		/// <param name="toDate">Right bound of the period (inclusive)</param>
		Task<IEnumerable<Guid>> GetAdvocatesToInvoice(DateTime fromDate, DateTime toDate);

		/// <summary>
		/// Return selected category for a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <returns>Selected category for a ticket.</returns>
		Task<Category> GetCategory(Guid ticketId);

		/// <summary>
		/// Returns escalated tickets that are waiting for diagnosis when escalated by a solver via tag.
		/// </summary>
		/// <param name="predicate">Condition to filter ticket.</param>
		/// <returns>List of escalated tickets that are waiting for diagnosis when escalated by a solver via tag.</returns>
		Task<IEnumerable<Ticket>> GetEscalatedDiagnosisEnabledTicket(Expression<Func<Ticket, bool>> predicate);

		/// <summary>
		/// Get probing form results for ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<IEnumerable<ProbingResult>> GetProbingResults(Guid ticketId);

		/// <summary>
		/// Get Customer Tickets
		/// </summary>
		/// <returns>List of customer tickets.</returns>
		Task<List<Ticket>> GetCustomerTickets(Guid customerId);

		/// <summary>
		/// Set ticket to ready state
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task SetTicketReady(Guid ticketId);

		/// <summary>
		/// Return a list of items, each item represent a brand and the number of tickets and total tickets price for that brand.
		/// All items belong to the specified advocate.
		/// </summary>
		/// <param name="invoicingCycleId">The invoice cycle id that tickets fall into</param>
		/// <returns>priceTotal is the sum of all ticket prices, ticketsCount is the number of tickets for each brand</returns>
		Task<List<(Guid brandId, string brandName, decimal priceTotal, int ticketsCount)>> GetTicketsWithBrandNameForAdvocateInvoice(DateTime fromDate, DateTime toDate, Guid advocateId);
	}
}
