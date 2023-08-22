using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Enums;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Export;
using Tigerspike.Solv.Application.Models.Search;
using Tigerspike.Solv.Application.Models.Statistics;
using Tigerspike.Solv.Application.Models.Ticket;
using Tigerspike.Solv.Core.Models;
using Tigerspike.Solv.Core.Models.PagedList;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface ITicketService
	{
		/// <summary>
		/// Retunrs the list of tickets the are assigned to the passed advocate id.
		/// </summary>
		/// <param name="advocateId">The id of advocate</param>
		/// <param name="userLevel">The advocate level.</param>
		/// <returns>List of ticket model</returns>
		Task<IEnumerable<TicketModel>> GetAdvocateTickets(Guid advocateId, TicketLevel userLevel);

		/// <summary>
		/// Creates a new ticket.
		/// </summary>
		/// <param name="ticketCreateModel">The ticket create model.</param>
		/// <param name="brandId">The brand id.</param>
		/// <returns>The id of the newly created ticket.</returns>
		Task<Guid?> Submit(CreateTicketModel ticketCreateModel, Guid brandId);

		/// <summary>
		/// Attempts to reserve a new ticket for the user with the specified id.
		/// </summary>
		/// <param name="advocateId">The advocate id to reserve a ticket for.</param>
		/// <returns>The ticket id if reserved, null otherwise.</returns>
		Task<Guid?> Reserve(Guid advocateId, bool isSuperSolver);

		/// <summary>
		/// Set spcified tags on the ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		/// <param name="tagIds">The list of tag ids to set</param>
		/// <param name="userLevel">Determines user level.</param>
		Task SetTags(Guid ticketId, Guid[] tagIds, TicketLevel? userLevel);

		/// <summary>
		/// Accept a ticket to be assigned for the advocate who reserved the ticket already. The
		/// advocate should haven't passed the deadline for the ticket reservation period.
		/// </summary>
		/// <param name="ticketId">The ticket id to be accepted</param>
		Task Accept(Guid ticketId);

		/// <summary>
		/// Transition the ticket to next state, based upon the ticket state
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		Task<string> Transition(Guid ticketId);

		/// <summary>
		/// The customer refused to close it after the advocate marked it as solved. It will return
		/// to Assigned status, so advocate can finish working on it.
		/// </summary>
		Task Reopen(Guid ticketId);

		/// <summary>
		/// The customer asked to close the ticket.
		/// </summary>
		/// <param name="ticketId">The ticket to be closed.</param>
		/// <param name="closedBy">who closed the ticket.</param>
		Task Close(Guid ticketId, ClosedBy closedBy = ClosedBy.Customer);

		/// <summary>
		/// The advocate/super solve asked to complete the tagging of the closed.
		/// </summary>
		/// <param name="ticketId">The ticket to be closed.</param>
		/// <param name="tagStatus">The tagging status of the ticket.</param>
		Task Complete(Guid ticketId, TicketTagStatus tagStatus);

		/// <summary>
		/// The advocate rated the ticket with the complexity.
		/// </summary>
		/// <param name="ticketId">The ticket identifier</param>
		/// <param name="complexity">The complexity value</param>
		Task SetComplexity(Guid ticketId, int complexity);

		/// <summary>
		/// The customer rated the ticket CSAT.
		/// </summary>
		/// <param name="ticketId">The ticket identifier</param>
		/// <param name="csat">The csat value</param>
		Task SetCsat(Guid ticketId, int csat);

		/// <summary>
		/// Return the ticket information with the specified id.
		/// </summary>
		/// <param name="ticketId">the ticket identifier</param>
		/// <param name="level">the Access Level</param>
		/// <returns>TicketModel object if exists</returns>
		Task<TicketModel> GetTicket(Guid ticketId, AccessLevel level);

		/// <summary>
		/// Returns a list of all available reject reasons.
		/// </summary>
		Task<IEnumerable<RejectReasonModel>> GetRejectReasons();

		/// <summary>
		/// Determines if the user can view the tickets.
		/// </summary>
		/// <param name="user">The user principal.</param>
		/// <param name="ticketIds">The one or more ticket ids.</param>
		/// <returns>True if the user can view the ticket, false otherwise.</returns>
		Task<bool> CanView(ClaimsPrincipal user, params Guid[] ticketIds);

		/// <summary>
		/// Determines if the user can edit the tickets.
		/// Meaning, the ticket is in a status that allows changing specific values
		/// such as adding messages, rating CSAT, rating complexity ..etc
		/// </summary>
		/// <param name="user">The user principal.</param>
		/// <param name="ticketIds">The one or more ticket ids.</param>
		/// <returns>True if the user can edit the ticket, false otherwise.</returns>
		Task<bool> CanEdit(ClaimsPrincipal user, SolvOperationEnum operation, Guid[] ticketIds);

		/// <summary>
		/// Gets ticket status statistics for all closed tickets
		/// </summary>
		Task<TicketStatisticByStatusModel> GetStatisticsByStatusForAll();

		/// <summary>
		/// Gets ticket status statistics for all closed tickets in specified brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<TicketStatisticByStatusModel> GetStatisticsByStatusForBrand(Guid brandId);

		/// <summary>
		/// Gets ticket status statistics for all closed tickets for specified advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<TicketStatisticByStatusModel> GetStatisticsByStatusForAdvocate(Guid advocateId);

		/// <summary>
		/// Gets ticket statistics overview for all tickets
		/// </summary>
		/// <param name="from">Optional starting date range</param>
		/// <param name="to">Optional ending date range</param>
		Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForAll(DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Gets ticket statistics overview for all tickets in specified brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="from">Optional starting date range</param>
		/// <param name="to">Optional ending date range</param>
		Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForBrand(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Get the ticket information that a customer is allowed to see.
		/// </summary>
		Task<CustomerTicketModel> GetCustomerTicket(Guid ticketId);

		/// <summary>
		/// Gets ticket statistics overview for all tickets for specified advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <param name="brandId">The brand id (optional)</param>
		/// <param name="from">Optional starting date range</param>
		/// <param name="to">Optional ending date range</param>
		Task<TicketStatisticsPerformanceModel> GetStatisticsPerformanceOverview(Guid advocateId, Guid? brandId, DateTime? from = null);

		/// <summary>
		/// Gets ticket statistics overview for all tickets for specified advocate
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		/// <param name="from">Optional starting date range</param>
		/// <param name="to">Optional ending date range</param>
		Task<TicketStatisticsOverviewModel> GetStatisticsOverviewForAdvocate(Guid advocateId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Returns statistics for closed ticket for specified period
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="fromDate">Left bound of the period (inclusive)</param>
		/// <param name="toDate">Right bound of the period (inclusive)</param>
		/// <returns></returns>
		Task<TicketStatisticsForBillingCycleModel> GetStatisticsForPeriod(Guid brandId, DateTime fromDate, DateTime toDate);

		/// <summary>
		/// Returns statistics for closed ticket for the current period
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <returns></returns>
		Task<TicketStatisticsForBillingCycleModel> GetStatisticsForCurrentPeriod(Guid brandId);

		/// <summary>
		/// Returns statistics for escalated tickets by source
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="from">An optional starting date (must also then pass a value to the to parameter 'to', to work)</param>
		/// <param name="to">An optional ending date (will be used to create a date range, with the previous 'from' paramter)</param>
		/// <returns></returns>
		Task<TicketStatisticsForEscalatedModel> GetStatisticsForEscalated(Guid brandId, DateTime? from = null, DateTime? to = null);

		/// <summary>
		/// Returns whether abandoned threshold was reached and ticket should be escalated
		/// </summary>
		/// <param name="ticket">The ticket to check</param>
		/// <param name="escalationConfig">Escalation config</param>
		bool IsEscalationAbandonedThresholdReached(Ticket ticket, TicketEscalationConfig escalationConfig);

		/// <summary>
		/// Returns whether rejection threshold was reached and ticket should be escalated
		/// </summary>
		/// <param name="ticket">The ticket to check</param>
		/// <param name="escalationConfig">Escalation config</param>
		bool IsEscalationRejectionThresholdReached(Ticket ticket, TicketEscalationConfig escalationConfig);

		/// <summary>
		/// Returns whether timeout threshold was reached and ticket should be escalated
		/// </summary>
		/// <param name="ticket">The ticket to check</param>
		/// <param name="escalationConfig">Escalation config</param>
		bool IsEscalationTimeoutReached(Ticket ticket, TicketEscalationConfig escalationConfig);

		/// <summary>
		/// Get ticket search model data
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<TicketSearchModel> GetSearchModel(Guid ticketId);

		/// <summary>
		/// Returns whether the tickets was too long in the Solved state and should be closed now
		/// </summary>
		/// <param name="ticket">Ticket (with StatusHistory)</param>
		bool ShouldTicketBeClosed(Ticket ticket);

		/// <summary>
		/// Gets dashboard statistics
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<AdvocateStatisticPeriodSummaryModel> GetStatisticsPeriodPackage(Guid advocateId);

		/// <summary>
		/// Gets flat data to export for the tickets in specified period
		/// </summary>
		/// <param name="ticketCsvExportParameterModel">The csv export parameters</param>
		Task<Stream> GetExportData(TicketCsvExportParameterModel ticketCsvExportParameterModel);

		/// <summary>
		/// Triggers manual abandon for a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <param name="reasonIds">The reasons the ticket was abandoned.</param>
		Task Abandon(Guid ticketId, Guid[] reasonIds);

		/// <summary>
		/// Triggers manual reject for a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		/// <param name="reasonIds">The reasons the ticket was rejected.</param>
		Task Reject(Guid ticketId, int[] reasonIds);

		/// <summary>
		/// Triggers manual forced escalation.
		/// </summary>
		/// <param name="ticketId">The ticket id.</param>
		Task Escalate(Guid ticketId);

		/// <summary>
		/// Get the number of available tickets for the selected advocate
		/// </summary>
		Task<int> GetAvailableTickets(Guid advocateId, TicketLevel level);

		/// <summary>
		/// Get ticket tags
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		/// <param name="userLevel">The user level</param>
		Task<IEnumerable<TagModel>> GetTags(Guid ticketId, TicketLevel userLevel);

		/// <summary>
		/// The customer rated the ticket NPS.
		/// </summary>
		/// <param name="ticketId">The ticket identifier</param>
		/// <param name="nps">The NPS value</param>
		Task SetNps(Guid ticketId, int nps);

		/// <summary>
		/// Get advocate performance statistics for a given period.
		/// </summary>
		/// <param name="advocateId">Advocate identifier.</param>
		/// <param name="period">Get data for week / month/ year.</param>
		/// <param name="time">Selected timestamp.</param>
		/// <param name="brandIds">List of brand id's.</param>
		/// <returns>Model of type AdvocatePerformanceStatisticPeriodSummaryModel.</returns>
		Task<AdvocatePerformanceStatisticPeriodSummaryModel> GetAdvocatePerformanceStatisticsPeriod(Guid advocateId, string period, DateTime? time, Guid[] brandIds);

		/// <summary>
		/// Set notification resumption state on the ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task MarkResumption(Guid ticketId);

		/// <summary>
		/// Remove sensitive information as per access level
		/// </summary>
		/// <param name="level">The desired access level</param>
		/// <param name="tickets">Tickets to process</param>
		Task<IEnumerable<TicketModel>> TrimSensitiveInformation(AccessLevel level, params TicketModel[] tickets);

		/// <summary>
		/// Returns import brand ticket list
		/// </summary>
		/// <returns>import brand ticket list</returns>
		Task<IPagedList<TicketImportModel>> GetImportTicket(Guid brandId, int pageIndex = 0,
			int pageSize = 25, TicketImportSortBy sortBy = TicketImportSortBy.uploadDate, SortOrder sortOrder = SortOrder.Desc);

		/// <summary>
		/// Returns Import failure tickets
		/// </summary>
		/// <returns>Import failure tickets</returns>
		Task<string> GetAllFailureImportTicket(Guid ticketImportid);

		/// <summary>
		/// Set Spos for the specific ticket
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <param name="model">The ticket spos model.</param>
		Task SetSpos(Guid ticketId, TicketSposModel model);

		/// Set category for a ticket.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <param name="categoryId">The category identifier.</param>
		/// <param name="advocateId">The advocate identifier.</param>
		/// <returns></returns>
		Task SetCategory(Guid ticketId, Guid categoryId, Guid advocateId);

		/// <summary>
		/// Returns category set for a specific ticket.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <returns>Category set for a specific ticket.</returns>
		Task<CategoryModel> GetCategory(Guid ticketId);

		/// <summary>
		/// Set Diagnosis for the specific ticket.
		/// </summary>
		/// <param name="ticketId">The ticket identifier.</param>
		/// <param name="model">The ticket diagnosis model.</param>
		Task<bool> SetDiagnosis(Guid ticketId, TicketDiagnosisModel model);

		/// <summary>
		/// Set valid transfer for specific ticket.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="model">Ticket valid transfer model.</param>
		/// <returns>Returns if the call was success or failure.</returns>
		Task<bool> SetValidTransfer(Guid ticketId, TicketValidTransferModel model);

		/// <summary>
		/// Set additional feedback for specific ticket.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="additionalFeedBack">Additional FeedBack.</param>
		/// <returns></returns>
		Task SetAdditionalFeedBack(Guid ticketId, string additionalFeedBack);

		/// <summary>
		/// The method to get the disabled the tags of the ticket brand
		/// </summary>
		/// <param name="ticket"></param>
		/// <returns></returns>
		Task<List<Guid>> GetDisabledTagsOfTicketBrand(Ticket ticket);

		(TicketFlowAction? action, string value) GetProbingEvaluation(IEnumerable<ProbingResult> probingResults, ProbingForm probingForm = null);
	}
}