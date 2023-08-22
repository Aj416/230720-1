
namespace Tigerspike.Solv.Api.Models.Advocate
{
	/// <summary>
	/// The response for /paypal-redirect-url endpoint
	/// </summary>
	public class AdvocatePayPalUrlResponseModel
	{
		/// <summary>
		/// The PayPal url that the advocate will be redirected to, to authorize Solv on PayPal
		/// </summary>
		public string Url { get; set; }
	}
}
