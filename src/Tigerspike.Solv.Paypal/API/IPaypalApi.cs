using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Refit;

namespace Tigerspike.Solv.Paypal.API
{
	public interface IPaypalApi
	{
		[Post("/v1/oauth2/token")]
		Task<AuthToken> GetToken([Header("Authorization")] string authorization, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Body] string body);

		[Post("/v1/customer/partner-referrals")]
		[Headers("Content-Type: application/json")]
		Task<string> GetPartnerReferrals([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Body] string body);

		[Get("/v1/customer/partners/{payerId}/merchant-integrations?tracking_id={trackingId}")]
		[Headers("Content-Type: application/json")]
		Task<string> CheckSolverStatusByTrackingId([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Query] string payerId, [Query] string trackingId);

		[Get("/v1/customer/partners/{payerId}/merchant-integrations/{merchantId}")]
		[Headers("Content-Type: application/json")]
		Task<string> ValidateSolverPermissions([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Query] string payerId, [Query] string merchantId);

		[Post("/v1/billing-agreements/agreement-tokens")]
		[Headers("Content-Type: application/json")]
		Task<string> GetBillingAgreementToken([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Body] string body);

		[Post("/v1/billing-agreements/agreements")]
		[Headers("Content-Type: application/json")]
		Task<string> FinalizeBillingAgreement([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Body] string body);

		/// <summary>
		/// Orders PayPal to create a payment to be executed (capture) later.
		/// </summary>
		[Post("/v2/checkout/orders")]
		[Headers("Content-Type: application/json")]
		Task<string> CreatePayment([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Body] string body);

		/// <summary>
		/// Get the payment that was created by the previous call to check the status.
		/// </summary>
		[Headers("Content-Type: application/json")]
		[Get("/v2/checkout/orders/{orderId}")]
		Task<string> GetPaymentDetail([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Query] string orderId);

		/// <summary>
		/// Execute ('Capture' in PayPal terms) the payment that was created so the money get transferred for real.
		/// </summary>
		[Post("/v2/checkout/orders/{orderId}/capture")]
		[Headers("Content-Type: application/json")]
		Task<string> ExecutePayment([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Header("PayPal-Request-Id")] string paypalRequestId, [Header("PayPal-Client-Metadata-Id")] string trackingId, [Query] string orderId, [Body] string body);

		/// <summary>
		/// Creates or updates the transaction context, by merchant ID and tracking ID
		/// </summary>
		[Put("/v1/risk/transaction-contexts/{merchantId}/{trackingId}")]
		[Headers("Content-Type: application/json")]
		Task SetRiskTransactionContext([Header("Authorization")] string token, [Header("PayPal-Partner-Attribution-Id")] string bnCode, [Query] string merchantId, [Query] string trackingId, [Body] string body);
	}
}