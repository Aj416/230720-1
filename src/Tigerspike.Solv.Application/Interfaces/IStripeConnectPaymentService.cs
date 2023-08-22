using System.Threading.Tasks;

namespace Tigerspike.Solv.Application.Interfaces
{
	/// <summary>
	/// A service interface for Stripe payment service
	/// </summary>
	public interface IStripePaymentService
	{
		/// <summary>
		/// Create an express account in Stripe
		/// </summary>
		/// <param name="authorizationCode">The authorization code returned by Stripe after authorization</param>
		/// <returns>The account id</returns>
		Task<string> FinaliseExpressAccount(string authorizationCode);
	}
}
