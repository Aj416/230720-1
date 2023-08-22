
using System;

namespace Tigerspike.Solv.Application.SignalR
{
	internal static class TicketHubConstants
	{
		// Inidicates that a ticket has just been added to the advocate pool.
		public const string TICKET_ADDED = nameof(TICKET_ADDED);

		// Inidicates that a ticket has just been removed from the advocate pool.
		public const string TICKET_REMOVED = nameof(TICKET_REMOVED);

		internal const string ADVOCATE_ONLINE = nameof(ADVOCATE_ONLINE);
		internal const string ADVOCATE_OFFLINE = nameof(ADVOCATE_OFFLINE);
		internal const string ADVOCATE_PUSH_ONLINE_LIST = nameof(ADVOCATE_PUSH_ONLINE_LIST);
		internal const string ADVOCATE_STATISTICS_UPDATED = nameof(ADVOCATE_STATISTICS_UPDATED);

		internal const string BRAND_AVAILABLE_TICKETS_UPDATED = nameof(BRAND_AVAILABLE_TICKETS_UPDATED);

		internal const string PENDING_TICKET_ADDED = nameof(PENDING_TICKET_ADDED);
		internal const string PENDING_TICKET_REMOVED = nameof(PENDING_TICKET_REMOVED);
		internal const string TICKET_TAGS_DISABLED_ADDED = nameof(TICKET_TAGS_DISABLED_ADDED);
		internal const string TAGGING_STATUS_CHANGED = nameof(TAGGING_STATUS_CHANGED);

		// Group names
		public static string GetBrandGroupName(Guid brandId) => string.Format("ADVOCATE_ONLINE_GROUP_{0}", brandId);
	}
}
