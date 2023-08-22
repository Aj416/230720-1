using System;

namespace Tigerspike.Solv.Core.Constants
{
	public static class CacheKeys
	{
		// Brand keys
		public static readonly Func<Guid, string> BrandInfoKey = brandId => $"brand:{brandId}:info:4814";
		public static readonly Func<Guid, object, string> AvailableTicketsKey = (brandId, level) => $"brand:{brandId}:tickets:{level}:3664";
		public static readonly Func<Guid, string> AdvocateTouchedTicketsKey = advocateId => $"advocate:{advocateId}:touched-tickets:3664";
		public static readonly Func<Guid, string> AdvocateBrandsKey = advocateId => $"advocate:{advocateId}:brands";
		public static readonly Func<Guid, string> GetBrandSuperSolvers = brandId => $"brand:{brandId}:supersolvers";
		public static readonly Func<Guid, string> GetBrandResponsesKey = brandId => $"brand:{brandId}:responses:4792";

		// Chat conversation keys
		public static readonly string ConversationKeyPattern = "conversation:{0}:{1}";

		public static readonly Func<string, string> GetCustomerConnectionsKey = ticketId => $"ticket:{ticketId}:customer-connections";
		public static readonly Func<Guid, string> GetAdvocateConnectionsKey = advocateId => $"advocate:{advocateId}:connections";
		public static readonly Func<Guid, string> GetBrandOnlineAdvocatesKey = brandId => $"brand:{brandId}:online-advocates";
		public static readonly Func<Guid, string> GetBrandWhiteListKey = brandId => $"brand:{brandId}:messageWhitelist";

		// Statistics
		public static readonly Func<Guid, string> PreviousWeekStatisticsPeriodKey = (Guid advocateId) => $"advocate:{advocateId}:statistics-periods:week:previous";
		public static readonly Func<Guid, string> CurrentWeekStatisticsPeriodKey = (Guid advocateId) => $"advocate:{advocateId}:statistics-periods:week:current";
		public static readonly Func<Guid, string> AllTimeStatisticsPeriodKey = (Guid advocateId) => $"advocate:{advocateId}:statistics-periods:all-time";

		// Ticket
		public static readonly Func<Guid, string> GetTicketTransportTypeKey = ticketId => $"ticket:{ticketId}:transportType";
		public static readonly Func<Guid, string> GetReturningCustomerFlowContextKey = ticketId => $"ticket:{ticketId}:returning-customer-flow-context";
		public static readonly Func<Guid, string> GetNotificationResumptionFlowContextKey = ticketId => $"ticket:{ticketId}:notification-resumption-flow-context";
		public static readonly Func<Guid, Guid?, string> GetTicketTransportModelKey = (ticketId, advocateId) => $"ticket:{ticketId}:transportModel:{advocateId}";

		// Advocate
		public static readonly Func<Guid, DateTime, string, string, string> AdvocatePerformanceBreakDownKey = (Guid advocateId, DateTime from, string period, string brands) => $"advocate:{advocateId}:brandIds:{brands}:statistics-breakdown:{period}:from:{from}";
		public static readonly Func<Guid, DateTime, string, string, string> AdvocatePerformanceSummaryKey = (Guid advocateId, DateTime from, string period, string brands) => $"advocate:{advocateId}:brandIds:{brands}:statistics-summary:{period}:from:{from}";
		public static readonly Func<Guid, string> AdvocateInternalAgentInfoKey = userId => $"user:{userId}:internalAgent";

		// User
		public static readonly Func<Guid, string> UserEnabledKey = userId => $"user:{userId}:enabled";

	}
}
