namespace Tigerspike.Solv.Api.Models.Advocate
{
	/// <summary>
	/// Setup Payment Model
	/// </summary>
	public class AdvocateUpdatePaymentMethodModel
	{
		/// <summary>
		/// Indicates that the payment method has been setup
		/// </summary>
		public bool PaymentMethodSetup { get; set; }

		/// <summary>
		/// Indicates that the payment method email has been verified.
		/// </summary>
		public bool PaymentEmailVerified { get; set; }

		/// <summary>
		/// The payment account Id that was setup.
		/// </summary>
		public string PaymentAccountId { get; set; }
	}
}