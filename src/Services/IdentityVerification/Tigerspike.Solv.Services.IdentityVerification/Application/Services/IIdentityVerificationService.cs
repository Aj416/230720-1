using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tigerspike.Solv.Services.IdentityVerification.Application.Services
{
	public interface IIdentityVerificationService
	{
		/// <summary>
		/// Creates a new applicant profile.
		/// </summary>
		/// <param name="firstName">Applicant first name.</param>
		/// <param name="lastName">Applicant last name.</param>
		/// <returns>The applicant id.</returns>
		Task<string> CreateApplicant(string firstName, string lastName);

		/// <summary>
		/// Updates an existing applicant.
		/// </summary>
		/// <param name="applicantId">Applicant id.</param>
		/// <param name="firstName">Applicant first name.</param>
		/// <param name="lastName">Applicant last name.</param>
		/// <returns>The applicant id.</returns>
		Task<string> UpdateApplicant(string applicantId, string firstName, string lastName);

		/// <summary>
		/// Generates a token for a new or existing applicant.
		/// </summary>
		/// <param name="applicantId">The applicant id.</param>
		/// <returns>The generated token.</returns>
		Task<string> GenerateSdkToken(string applicantId);

		/// <summary>
		/// Creates a new check for the applicant identity.
		/// </summary>
		/// <param name="applicantId">The applicant id.</param>
		/// <returns>The check id and report url.</returns>
		Task<(string checkId, string reportUrl)> CreateCheck(string applicantId);

		/// <summary>
		/// Consume webhook notification
		/// </summary>
		/// <param name="request">Received request</param>
		/// <returns>Whether the webhook was properly processed or not</returns>
		Task<bool> ConsumeWebhook(HttpRequest request);
	}
}
