namespace Tigerspike.Solv.Application.Constants
{
	/// <summary>
	/// The workflow keys
	/// </summary>
	public static class WorkflowKeys
	{
		/// <summary>
		/// Key for create ticket workflow
		/// </summary>
		public static readonly string CreateTicketKey = "ticket-create";

		/// <summary>
		/// Key for complete ticket workflow
		/// </summary>
		public static readonly string CompleteTicketKey = "ticket-complete";

		/// <summary>
		/// Version of create ticket workflow
		/// </summary>
		public static readonly int CreateTicketVersion = 4245; // Use JIRA ticket id for any further changes

		/// <summary>
		/// Version of complete ticket workflow
		/// </summary>
		public static readonly int CompleteTicketVersion = 4548; // Use JIRA ticket id for any further changes

	}
}
