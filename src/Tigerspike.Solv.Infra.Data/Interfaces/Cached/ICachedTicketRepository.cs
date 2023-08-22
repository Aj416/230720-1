using System;
using System.Threading.Tasks;
using Tigerspike.Solv.Domain.Enums;
using Tigerspike.Solv.Infra.Data.Models;

namespace Tigerspike.Solv.Infra.Data.Interfaces.Cached
{
	public interface ICachedTicketRepository
	{
		/// <summary>
		/// Gets transport type of the ticket (cached)
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<TicketTransportType> GetTransportType(Guid ticketId);

		/// <summary>
		/// Get transport model data (cached)
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		/// <param name="advocateId">The advocate id (needed for caching purposes)</param>
		/// <returns></returns>
		Task<TicketTransportModel> GetTransportModel(Guid ticketId, Guid? advocateId);

		/// <summary>
		/// Gets context for processing Returning Customer flow for the ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<ReturningCustomerFlowContext> GetReturningCustomerFlowContext(Guid ticketId);

		/// <summary>
		/// Sets context for processing Returning Customer flow for the ticket
		/// </summary>
		/// <param name="context">The updated context</param>
		void SetReturningCustomerFlowContext(ReturningCustomerFlowContext context);

		/// <summary>
		/// Sets dashboard statistics
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<AdvocateStatisticPeriodSummaryModel> UpdateStatistics(Guid advocateId);

		/// <summary>
		/// Gets dashboard statistics
		/// </summary>
		/// <param name="advocateId">The advocate id</param>
		Task<AdvocateStatisticPeriodSummaryModel> GetStatisticsPeriodPackage(Guid advocateId);

		/// <summary>
		/// Get advocate performance statistics for a given period.
		/// </summary>
		/// <param name="advocateId">Advocate identifier.</param>
		/// <param name="from">From timestamp.</param>
		/// <param name="to">To timestamp.</param>
		/// <param name="brandIds">List of brand id's.</param>
		/// <param name="period">Time period - week / month / year.</param>
		/// <param name="advocateBrandIds">Advocate associated BrandIds.</param>
		/// <returns>Model of type AdvocatePerformanceStatisticPeriodSummaryModel.</returns>
		Task<AdvocatePerformanceStatisticPeriodSummaryModel> GetAdvocatePerformanceStatisticsPeriod(Guid advocateId, DateTime from, DateTime to, Guid[] brandIds, Period period, Guid[] advocateBrandIds);

		/// <summary>
		/// Gets context for processing Notification Resumption flow for the ticket
		/// </summary>
		/// <param name="ticketId">The ticket id</param>
		Task<NotificationResumptionFlowContext> GetNotificationResumptionFlowContext(Guid ticketId);

		/// <summary>
		/// Sets context for processing Notification Resumption flow for the ticket
		/// </summary>
		/// <param name="context">The updated context</param>
		void SetNotificationResumptionFlowContext(NotificationResumptionFlowContext context);
	}
}