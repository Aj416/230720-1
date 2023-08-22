namespace Tigerspike.Solv.Core
{
	public enum EmailTemplates
	{
		AdvocateAccepted,
		ExportAdvocateApplication,
		DeleteAdvocateApplication,
		AdvocateApplicationCreated,
		AdvocateApplicationCompleted,
		AdvocateApplicationProfiling,
		AdvocateNewBrandsAssigned,
		SuperSolverTicketEscalated,
		TicketCreated,
		FirstAdvocateResponseInChat,
		TicketClosed_Chat,

		/// <summary>
		/// a template for ticket (email transport) closed by system.
		/// </summary>
		TicketClosed_System_Email,

		/// <summary>
		/// a template for ticket (email transport) closed by customer.
		/// </summary>
		TicketClosed_Customer_Email,

		TicketSolved_Email,
		TicketSolved_Chat,
		TicketEscalated,
		TicketExport,
		AdvocateRepliedInChat,
		AdvocateRepliedInEmail,
		BrandApplicationReceived,
		TicketSposEmail,
		BrandsBlocked_Email,
		MfaReset,
	}
}