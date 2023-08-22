namespace Tigerspike.Solv.Api.Models.Brand
{
	/// <summary>
	/// The response for /paypal-redirect-url endpoint
	/// </summary>
	public class BrandPayPalUrlResponseModel
	{
		/// <summary>
		/// The PayPal url that the client (brand) will be redirected to, to authorize Solv on PayPal
		/// </summary>
		public string Url { get; set; }
	}
}
