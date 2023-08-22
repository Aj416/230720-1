namespace Tigerspike.Solv.Services.Chat.Enums
{
	public enum TicketStatusEnum
	{
		/// <summary>
		/// The ticket is brand new, and not yet assigned.
		/// </summary>
		New,

		/// <summary>
		/// The ticket is reserved, but not yet assigned.
		/// </summary>
		Reserved,

		/// <summary>
		/// The ticket is assigned to an advocate.
		/// </summary>
		Assigned,

		/// <summary>
		/// The ticket is makred as solved by advocate, but not yet closed by customer.
		/// </summary>
		Solved,

		/// <summary>
		/// The customer accepted to close the ticket and it is closed now.
		/// </summary>
		Closed,

		/// <summary>
		/// The ticket could not be solved in a timely fashion on the platform, it is escalated back to the creator of the ticket
		/// </summary>
		Escalated,

		/// <summary>
		/// Not an actual ticket state, represents the status of tickets escalated by a solver via tags untill its diagnosed valid or invalid.
		/// </summary>
		Pending
	}
}
