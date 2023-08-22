namespace Tigerspike.Solv.Domain.Enums
{
	public enum FraudLevel
	{
		/// <summary>
		/// The ticket is regular level
		/// </summary>
		L1 = 1,

		/// <summary>
		/// The ticket is escalated, ready to be picked up by the SuperSolver
		/// </summary>
		L2 = 2,

		/// <summary>
		/// The ticket is pushed back (escalated) to the external system (like client's CRM)
		/// </summary>
		L3 = 3,
	}
}