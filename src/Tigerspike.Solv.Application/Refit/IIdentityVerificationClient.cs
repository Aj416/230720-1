using System;
using System.Threading.Tasks;
using Refit;

namespace Tigerspike.Solv.Application.Refit
{
	public interface IIdentityVerificationClient
	{
		/// <summary>
		/// Gets the conversation for the provided id.
		/// </summary>
		[Get("/token")]
		[Headers("Content-Type:application/json")]
		Task<string> GenerateSdkToken([Query] Guid advocateId, [Query] string applicantId, [Query] string firstName,
			[Query] string lastName);

		/// <summary>
		/// Gets the conversation for the provided id.
		/// </summary>
		[Post("/webhook")]
		[Headers("Content-Type:application/json")]
		Task ConsumeIdentityCheckWebhook([Body] string payload, [Header("x-sha2-signature")] string signature);
	}
}