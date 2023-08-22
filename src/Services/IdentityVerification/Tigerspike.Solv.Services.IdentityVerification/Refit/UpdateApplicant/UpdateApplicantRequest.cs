namespace Tigerspike.Solv.Services.IdentityVerification.Refit.UpdateApplicant
{
	public class UpdateApplicantRequest
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public UpdateApplicantRequest(string firstName, string lastName)
		{
			FirstName = firstName;
			LastName = lastName;
		}
	}
}