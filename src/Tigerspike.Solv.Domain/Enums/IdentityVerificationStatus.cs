namespace Tigerspike.Solv.Domain.Enums
{
	public enum IdentityVerificationStatus
	{
		Pending = 0, // Onfido verification is not submitted
		Processing = 1, // Onfido verification is submitted and pending
		Failed = 2, // Onfido verification is submitted and failed
		Completed = 3, // Onfido verification is submitted and successful
	}
}