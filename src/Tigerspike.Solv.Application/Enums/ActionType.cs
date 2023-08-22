namespace Tigerspike.Solv.Application.Enums
{
	/// <summary>
	/// Types of chat actions
	/// </summary>
	public enum ActionType
	{
		/// <summary>
		/// "Is ticket actually solved?"
		/// </summary>
		IsTicketSolvedQuestion = 1,

		/// <summary>
		/// "Please rate you experience"
		/// </summary>
		CSAT = 2,

		/// <summary>
		/// Net Promoter Score dialog
		/// </summary>
		NPS = 3,

		/// <summary>
		/// Manually escalate ticket from chat
		/// </summary>
		Escalate = 4,

		/// <summary>
		/// Additional feedback dialog
		/// </summary>
		FeedBack = 5
	}
}