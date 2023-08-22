namespace Tigerspike.Solv.Paypal
{
	public class PaypalOptions
	{
		public const string SectionName = "Paypal";

		/// <summary>
		/// The Url of PayPal api.
		/// </summary>
		public string ApiUrl { get; set; }

		/// <summary>
		/// The PayPal App client Id
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// The PayPal App client secret
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// The PayPal partner id (merchant id) of Solv account (used to call PayPal api only)
		/// </summary>
		public string CallerPartnerId { get; set; }

		/// <summary>
		/// The PayPal merchant id of a Solv account  (used for receiving Solv fees from clients)
		/// </summary>
		public string PaymentReceiverAccountId { get; set; }

		/// <summary>
		/// The logo that PayPal will use of the partner (Solv in our case)
		/// </summary>
		public string PartnerLogo { get; set; }

		/// <summary>
		/// The solver return url after signing in/up to PayPal.
		/// </summary>
		public string SolverReturnUrl { get; set; }

		/// <summary>
		/// The client return url after signing in/up to PayPal.
		/// </summary>
		public string ClientReturnUrl { get; set; }

		/// <summary>
		/// Build notion code supplied by PayPal team.
		/// </summary>
		/// <value></value>
		public string BNCode { get; set; }
	}
}