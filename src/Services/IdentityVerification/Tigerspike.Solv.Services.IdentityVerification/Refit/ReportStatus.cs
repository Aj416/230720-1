namespace Tigerspike.Solv.Services.IdentityVerification.Refit
{
	public enum CheckStatus
	{
		InProgress, // We are currently processing the check.
		AwaitingApplicant, // Applicant has not yet submitted the applicant form, either because they have not started filling the form out or because they have started but have not finished.
		Complete, // All reports for the applicant have been completed or withdrawn.
		Withdrawn, // Check has been withdrawn.
		Paused, // Check is paused until you, i.e. the client, switch it on manually. Special case used by clients who wants to collect data and run the checks when they want and not immediately.
		Reopened, // Insufficient/inconsistent information is provided by the applicant, and the report has been bounced back for further information.
	}
}