using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tigerspike.Solv.Application.Services
{
	public interface IIdentityVerificationService
	{
		/// <summary>
		/// Creates a new identity check.
		/// </summary>
		/// <param name="advocateId"></param>
		/// <returns></returns>
		Task CreateCheck(Guid advocateId);

		/// <summary>
		/// Generates the sdk for the advocate.
		/// </summary>
		/// <param name="advocateId">The advocate id.</param>
		/// <returns></returns>
		Task<string> GenerateSdkToken(Guid advocateId);

		/// <summary>
		/// Consume webhook notification
		/// </summary>
		/// <param name="request">Received request</param>
		/// <returns>Whether the webhook was properly processed or not</returns>
		Task ConsumeIdentityCheckWebhook(HttpRequest request);
	}
}