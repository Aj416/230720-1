namespace Tigerspike.Solv.Domain.Enums
{
	public enum TicketLevel
	{
		/// <summary>
		/// The ticket is regular level
		/// </summary>
		Regular = 1,

		/// <summary>
		/// The ticket is escalated, ready to be picked up by the SuperSolver
		/// </summary>
		SuperSolver = 2,

		/// <summary>
		/// The ticket is pushed back (escalated) to the external system (like client's CRM)
		/// </summary>
		PushedBack = 3,
	}
}
