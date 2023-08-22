using System.Threading.Tasks;
using Refit;
using Tigerspike.Solv.Services.IdentityVerification.Refit.CreateApplicant;
using Tigerspike.Solv.Services.IdentityVerification.Refit.CreateCheck;
using Tigerspike.Solv.Services.IdentityVerification.Refit.GenerateSdkToken;
using Tigerspike.Solv.Services.IdentityVerification.Refit.GetCheck;
using Tigerspike.Solv.Services.IdentityVerification.Refit.UpdateApplicant;

namespace Tigerspike.Solv.Services.IdentityVerification.Refit
{
	public interface IOnfidoApi
	{
		[Put("/v3/applicants/{applicantId}")]
		[Headers("Content-Type:application/json")]
		Task<UpdateApplicantResponse> UpdateApplicant([Header("Authorization")] string token, [Query] string applicantId, [Body] UpdateApplicantRequest input);

		[Post("/v3/applicants")]
		[Headers("Content-Type:application/json")]
		Task<CreateApplicantResponse> CreateApplicant([Header("Authorization")] string token, [Body] CreateApplicantRequest input);

		[Post("/v3/sdk_token")]
		[Headers("Content-Type:application/json")]
		Task<GenerateSdkTokenResponse> GenerateSdkToken([Header("Authorization")] string token, [Body] GenerateSdkTokenRequest input);

		[Post("/v3/checks")]
		[Headers("Content-Type:application/json")]
		Task<CreateCheckResponse> CreateCheck([Header("Authorization")] string token, [Body] CreateCheckRequest input);

		[Get("/v3/checks/{checkId}")]
		[Headers("Content-Type:application/json")]
		Task<GetCheckResponse> GetCheck([Header("Authorization")] string token, [Query] string checkId);

	}
}